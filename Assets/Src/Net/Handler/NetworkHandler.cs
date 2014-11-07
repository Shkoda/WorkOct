using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Src.Net.Envelopes.Server;
using Assets.Src.Threading;
using WorkOct.Net.Envelopes;
using WorkOct.Protocol;

namespace Assets.Src.Net.Handler
{
    internal class NetworkHandler
    {

        /// <summary>
        ///     When Reset it blocks processing of server queue, i.e. does not allow packets to be send over network
        /// </summary>
        private static readonly AutoResetEvent allowServerQueueProcessingEvent = new AutoResetEvent(false);

        /// <summary>
        ///     When Reset it blocks processing of server queue. It blocks it when there are no packets to send, and
        ///     sending thread doesn't take any cpu time.
        /// </summary>
        private static readonly ManualResetEvent hasNewServerEnvelopesEvent = new ManualResetEvent(false);

        /// <summary>
        ///     Queue of packets, received from server, waiting to be processed in main thread.
        /// </summary>
        private static readonly Queue<ClientEnvelope> clientQueue;

        /// <summary>
        ///     Queue of packets, that should be sent to server. Sending works in another thread and is non-blocking
        /// </summary>
        private static readonly Queue<ServerEnvelope> serverQueue;

        private static bool acceptIncomingPackets;

        private static bool acceptOutgoingPackets;

        /// <summary>
        ///     Receiving thread
        /// </summary>
        private static Thread clientReceivingThread;

        /// <summary>
        ///     Thread that sends data to server asynchronously
        /// </summary>
        private static Thread serverSendingThread;

        /// <summary>
        ///     Bool flag that is to true, when queue is being processed
        /// </summary>
        private static volatile bool serverQueueProcessing;

        /// <summary>
        ///     Reference to low-level network interface
        /// </summary>
        private static INetworkInterface httpNetwork;

        public static event Action LostConnection = delegate { };

        public static event Action ServerError = delegate { };


        /// <summary>
        ///     Current state of network to allow packets filtering (don't send SUpdates, when login is not logged in, for example)
        /// </summary>
        public static NetworkState State { get; set; }

        static NetworkHandler()
        {
            State = NetworkState.NotConnected;
            clientQueue = new Queue<ClientEnvelope>();
            serverQueue = new Queue<ServerEnvelope>();
        }

        public static void ConnectAndSendVersion(string address, int port, bool secure)
        {
            Debugger.Log(string.Format("Connecting to server on {0}:{1}...", address, port));
            if (State != NetworkState.NotConnected)
            {
                Debugger.Log("Already connected!", DebugType.NetworkHandler);
                return;
            }

            //start the thread that is sending packets to server
            serverQueueProcessing = true;
            serverSendingThread = new Thread(ProcessingServerQueue) {IsBackground = true};
            serverSendingThread.Start();

            acceptIncomingPackets = true;
            acceptOutgoingPackets = true;

            //throws
            httpNetwork = new TcpNetworkInterface();
            Debugger.Log("httpNetwork == " + httpNetwork);


            httpNetwork.Connect(address, port, secure);

            new SPingEnvelope().Send();
        }

        private static void httpNetwork_ServerError()
        {
            ServerError();
        }

        private static void httpNetwork_LostConnection()
        {
            LostConnection();
        }

        /// <summary>
        ///     Changes state of network
        /// </summary>
        /// <param name="state"></param>
        public static void ChangeState(NetworkState state)
        {
            State = state;
            Debugger.Log("Changed state to " + state, DebugType.NetworkHandler);
        }

        /// <summary>
        ///     Receives envelope from underlying NetworkConnection
        /// </summary>
        /// <param name="envelope"></param>
        internal static void Receive(ClientEnvelope envelope)
        {
            if (acceptIncomingPackets)
            {
//                Debugger.Log(" >> " + envelope.GetType().Name);

                lock (clientQueue)
                {
                    clientQueue.Enqueue(envelope);
                }
            }
        }

        /// <summary>
        ///     Sends envelope to sending queue for later sending
        /// </summary>
        /// <param name="envelope">Envelope with packet to send over</param>
        /// <returns>true if envelope was added for processing, false otherwise</returns>
        public static bool Send(ServerEnvelope envelope)
        {
            if (!acceptOutgoingPackets)
            {
                return false;
            }

            lock (serverQueue)
            {
                //and maybe here
                serverQueue.Enqueue(envelope);
                hasNewServerEnvelopesEvent.Set();
            }
            return true;
        }

        /// <summary>
        ///     Handles every stored packet in queue *in calling thread*
        /// </summary>
        public static void ProcessClientQueue()
        {
            if (clientQueue.Count > 0)
            {
                lock (clientQueue)
                {
                    ClientEnvelope envelope;
                    while (clientQueue.Count > 0)
                    {
                        envelope = clientQueue.Dequeue(); //no exception due to in-lock check above
                        envelope.Handle();
                    }
                }
            }
        }

        /// <summary>
        ///     Sends collected envelopes to server *in another thread* (non-blocking call)
        /// </summary>
        public static void ProcessServerQueue()
        {
            //setting auto-resetting event, so the queue can be one-time processed regardless of manual-resetting event
            //            allowServerQueueProcessingEvent.Set();
            ProcessServerQueueOnce();
        }

        /// <summary>
        ///     Thread that goes over every packet in server queue and send everything to server if possible
        /// </summary>
        private static void ProcessingServerQueue()
        {
            ThreadManager.RegisterThread("Processing Server Queue");

            while (serverQueueProcessing)
            {
                if (!hasNewServerEnvelopesEvent.WaitOne(1000) || !allowServerQueueProcessingEvent.WaitOne(1000))
                {
                    continue;
                }

                //                if (serverQueue.Count > 0 && udpNetwork.IsConnected)
                ProcessServerQueueOnce();
            }

            ThreadManager.UnRegisterThread();
        }

        private static void ProcessServerQueueOnce()
        {
            if (serverQueue.Count > 0)
            {
                lock (serverQueue)
                {
                    bool processed = false;
                    while (serverQueue.Count > 0)
                    {
                        ServerEnvelope envelope = serverQueue.Peek();

                        bool result = httpNetwork.SendEnvelope(envelope);

                        //if something goes wrong we try to send it later
                        if (!result)
                        {
                            //                                Disconnect(false);
                            //                                State = NetworkState.NotConnected;
                            break;
                        }

                        Debugger.Log(" << "+envelope.GetType().Name);

                        //remove sent packet from queue
                        serverQueue.Dequeue();
                        if (serverQueue.Count == 0)
                        {
                            processed = true;
                        }
                    }

                    if (processed)
                    {
                        hasNewServerEnvelopesEvent.Reset();
                    }
                }
            }
        }

        public static void ClearQueues()
        {
            lock (serverQueue)
            {
                serverQueue.Clear();
            }

            lock (clientQueue)
            {
                clientQueue.Clear();
            }
            Debugger.Log("Network queues cleared.", DebugType.NetworkHandler);
        }

        public static void Disconnect(bool wait)
        {
            //Do not accept any more packtes from and to server
            acceptIncomingPackets = false;
            acceptOutgoingPackets = false;

            //Then clear everything we have waiting for processing
            ClearQueues();

            //Stop any activity in MainThread
            //StopUdpHolePunching();

            //Disconnect Http
            Debugger.Log("Disconnecting Tcp", DebugType.NetworkHandler);

            //Send nonwaiting goodbye if interface was created up to this point
            if (httpNetwork != null)
            {
                httpNetwork.SendGoodbye();
                httpNetwork.StopReceiving(wait);
            }

            //Turn off processing of packets. Up to this point other thread should be looking at empty queue, hence end very soon
            hasNewServerEnvelopesEvent.Reset();
            serverQueueProcessing = false;

            //Wait for thread to end
            if (wait && serverSendingThread != null)
            {
                serverSendingThread.Join();
            }

            State = NetworkState.NotConnected;
        }

        public static void SetSessionKey(ulong sessionKey)
        {
            httpNetwork.SetSessionKey(sessionKey);
        }

        public static void StartPolling()
        {
            //            httpNetwork.SendEnvelope(new SPollEnvelope());
        }
    }
}