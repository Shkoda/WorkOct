using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Src.Net.Envelopes.Server;
using Assets.Src.Net.Handler;
using Assets.Src.Threading;
using ProtoBuf;
using WorkOct.Net;
using WorkOct.Net.Envelopes;

namespace Assets.Src.Net
{
    public class StreamStateEventArgs : EventArgs
    {
        public StreamState State { get; set; }
    }

    public class TcpNetworkInterface : INetworkInterface
    {
        /// <summary>
        ///     Event should be Set when connection to server is established and Reset, when it's lost. When being reset it
        ///     blocks receiving thread.
        /// </summary>
        private readonly ManualResetEvent receiveEvent = new ManualResetEvent(false);

        /// <summary>
        ///     Event should be Set when connection to server is lost. Reconnecting thread then is automaticly unblocked, and
        ///     starts probing network.
        /// </summary>
        private readonly ManualResetEvent reconnectEvent = new ManualResetEvent(false);

        private readonly PrecompiledSerializer serializer;
        private readonly object streamStateLocker = new object();
        private readonly NetworkTypesList typesList = new NetworkTypesList();
        private TcpClient client;
        private volatile StreamState currentStreamState;
        private volatile bool isReceiving;
        private NetworkStream networkStream;
        private Thread receivingThread;
        private Thread reconnectingThread;

        private string serverAddress;
        private int serverPort;
        private AutoResetEvent stopReceiveEvent;
        private bool wasConnected;

        public TcpNetworkInterface()
        {
            serializer = new PrecompiledSerializer();
        }

        public StreamState CurrentStreamState
        {
            get { return currentStreamState; }
            private set
            {
                StreamState temp = currentStreamState;
                lock (streamStateLocker)
                {
                    currentStreamState = value;
                }
                if (temp != value)
                {
                    StateChanged(this, new StreamStateEventArgs {State = currentStreamState});
                }
            }
        }

        public bool IsConnected { get; private set; }


        public void Connect(string address, int port, bool secure)
        {
            Connect(address, port);
        }

        public event Action LostConnection;
        public event Action ServerError;

        public void StopReceiving(bool wait)
        {
            Debugger.Log("Stop receiving Tcp", DebugType.NetworkInterface);
            isReceiving = false;

            CurrentStreamState = StreamState.Closed;
            StateChanged(this, new StreamStateEventArgs {State = CurrentStreamState});
            wasConnected = false;

            AbortConnection(wait);

            if (wait)
            {
                reconnectingThread.Join();
                Debugger.Log("Reconnecting thread down.", writeToUnityConsole: true);
                receivingThread.Join();
                Debugger.Log("Receiving thread down.", writeToUnityConsole: true);
            }
        }

        public void SendGoodbye()
        {
            Debugger.Log("SendGoodbye is not implemented in TcpNetworkInterface");

            var goodbyeEnvelope = new SPingEnvelope("bye!");
            networkStream.WriteByte((byte) goodbyeEnvelope.PacketType);
            serializer.SerializeWithLengthPrefix(networkStream,
                goodbyeEnvelope.Packet,
                typesList.ServerPacketTypesArray[(int) goodbyeEnvelope.PacketType],
                PrefixStyle.Fixed32BigEndian,
                0);

//            throw new System.NotImplementedException();

//            var goodbyeEnvelope = new SGoodbyeEnvelope();
//            networkStream.WriteByte((byte)goodbyeEnvelope.PacketType);
//            serializer.SerializeWithLengthPrefix(networkStream,
//                                                 goodbyeEnvelope.Packet,
//                                                 typesList.ServerPacketTypesArray[(int)goodbyeEnvelope.PacketType],
//                                                 PrefixStyle.Fixed32BigEndian,
//                                                 0);
//            Debugger.Log("Goodbye packet sent.", writeToUnityConsole: true);
        }

        public bool SendEnvelope(ServerEnvelope envelope)
        {
            try
            {
                SendMessage(envelope.Packet, envelope.PacketType,
                    typesList.ServerPacketTypesArray[(int) envelope.PacketType]);
            }
            catch (Exception e)
            {
                //                if (envelope.PacketType != ServerMessageType.SDEBUGMESSAGE)
                //                {
                Debugger.Log("Error while sending " + envelope.PacketType + ": " + e.Message, writeToUnityConsole: true);
                StartReconnect();
                //                }
                return false;
            }
            return true;
        }

        public void SetSessionKey(ulong sessionKey)
        {
        }

        public event EventHandler<StreamStateEventArgs> StateChanged = delegate { };

        /// <summary>
        ///     Creates connection to server at given address and port. Starts receiving and reconnecting threads.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SocketException"></exception>
        public void Connect(String address, int port)
        {
            Debugger.Log(string.Format("TcpNetworkInterface.Connect {0}:{1}...", address, port));
            serverAddress = address;
            serverPort = port;
            //return;
//            client = new TcpClient();

            isReceiving = true;

            reconnectingThread = new Thread(ReconnectThread) {IsBackground = true};
            reconnectingThread.Start();
            reconnectEvent.Set();

            receivingThread = new Thread(Receive);
            receivingThread.Start();
        }

        private void ReconnectThread()
        {
            ThreadManager.RegisterThread("Tcp reconnecting thread");

            while (isReceiving)
            {
                if (reconnectEvent.WaitOne(1000))
                {
                    Debugger.Log("Trying to (re)connect to server", DebugType.NetworkInterface);
                    if (TryConnect(serverAddress, serverPort))
                    {
                        Debugger.Log("Connection successfull", DebugType.NetworkInterface);
                        IsConnected = true;
                        reconnectEvent.Reset();
                        receiveEvent.Set();

                        CurrentStreamState = wasConnected ? StreamState.Reconnected : StreamState.Connected;

                        //Sending reconnect message if already connected and past logging in
                        if (wasConnected)
                        {
                            //                            MainThread.AddOnce(nothing => SendEnvelope(new SReconnectEnvelope()));
                        }

                        //wasConnected = true;
                    }
                    else
                    {
                        CurrentStreamState = StreamState.Reconnecting;
                        Debugger.Log("Retrying in 1000 ms", DebugType.NetworkInterface);
                        Thread.Sleep(1000);
                    }
                }
            }

            ThreadManager.UnRegisterThread();
        }

        private bool TryConnect(String address, int port)
        {
            try
            {
                Debugger.Log("TryConnect " + address + " : " + port);
                IPAddress ipadr = IPAddress.Any;

                IPAddress.TryParse(address, out ipadr);

                client = new TcpClient();
                client.Connect(address, port);
                networkStream = client.GetStream();
                client.Client.Blocking = true;
                //Read timeout so we can close receiving thread even if packets aren't coming
                //networkStream.ReadTimeout = 1;
            }

            catch (SocketException e)
            {
                Debugger.Log(e.NativeErrorCode, DebugType.Exception);
                Debugger.Log(e, DebugType.Exception);
                return false;
            }
            catch (Exception e)
            {
                Debugger.Log(e, DebugType.Exception);
                return false;
            }

            return true;
        }

        private void AbortConnection(bool wait)
        {
            try
            {
                //Throw away all data
                client.Client.LingerState = wait ? new LingerOption(true, 5) : new LingerOption(true, 0);
            }
            finally
            {
                try
                {
                    networkStream.Close();
                    networkStream.Dispose();
                    client.Close();
                }
                catch (Exception e)
                {
                    Debugger.Log(string.Format("Aborting exception: {0}", e.Message), writeToUnityConsole: true,
                        stackTrace: e.StackTrace);
                }
            }
        }

        private void Receive()
        {
            ThreadManager.RegisterThread("Tcp receiving");
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                while (isReceiving)
                {
                    Debugger.Log("Is Receiving");
                    if (receiveEvent.WaitOne(1000))
                    {
                        //Get message type
                        int messageType = 0;
                        try
                        {
                            messageType = networkStream.ReadByte();
                        }
                        catch (Exception e)
                        {
                            if (isReceiving)
                            {
                                Debugger.Log("Error while reading heading byte in TcpInterface: " + e.Message,
                                    DebugType.Exception, sendToServer: true, writeToUnityConsole: true);
                                StartReconnect();
                                break;
                            }
                        }

                        if (messageType == -1)
                        {
                            break;
                        }

                        if (messageType >= typesList.ClientPacketTypesArray.Length ||
                            typesList.ClientPacketTypesArray[messageType] == null)
                        {
                            //Clear the stream before throwing an exception
                            int receivedByte = 0;

                            try
                            {
                                while (receivedByte != -1)
                                {
                                    receivedByte = networkStream.ReadByte();
                                }
                            }
                            catch (Exception)
                            {
                                break;
                            }
                            finally
                            {
                                if (isReceiving)
                                {
                                    Debugger.Log(
                                        "Error: Received message is not in protocol. messageType = 0x" +
                                        messageType.ToString("X"),
                                        DebugType.NetworkInterface, sendToServer: true, writeToUnityConsole: true);
                                }
                            }
                        }

                        //Receiving packet
                        object cPacket = null;
                        try
                        {
#if OUTPUT_PACKET
                        if (messageType != 0x01)
                        {
#endif
                            cPacket = serializer.DeserializeWithLengthPrefix(networkStream, null,
                                typesList.ClientPacketTypesArray[messageType],
                                PrefixStyle.Fixed32BigEndian, 0);
#if OUTPUT_PACKET
                        }
                        else
                        {
                            var length = 0;
                            var reader = new BinaryReader(networkStream);
                            length = (reader.ReadByte() << 24) | (reader.ReadByte() << 16) | (reader.ReadByte() << 8) |
                                     (reader.ReadByte());

                            Debugger.Log(string.Format("packet length = {0}", length), writeToUnityConsole: true);
                            var buffer = new byte[length + 4];
                            var bytesReceived = networkStream.Read(buffer, 4, length);
                            Debugger.Log(string.Format("received length = {0}", bytesReceived), writeToUnityConsole: true);

                            buffer[0] = (byte) (0xFF & (length >> 24));
                            buffer[1] = (byte) (0xFF & (length >> 16));
                            buffer[2] = (byte) (0xFF & (length >> 8));
                            buffer[3] = (byte)(0xFF & length);

                            string output = "";
                            for (int i = 0; i < buffer.Length; i++)
                            {
                                output += "0x" + buffer[i].ToString("X") + ", ";
                            }

                            Debugger.Log(string.Format("received, bytes = {0}", output), writeToUnityConsole: true);

                            using (var memoryStream = new MemoryStream(buffer))
                            {
                                cPacket = serializer.DeserializeWithLengthPrefix(memoryStream, null,
                                                                                                 typesList.ClientPacketTypesArray[messageType],
                                                                                                 PrefixStyle.Fixed32BigEndian, 0);
                            }


                        }
#endif
                        }
                        catch (Exception e)
                        {
                            Debugger.Log(
                                string.Format("Exception while deserializing packet 0x{1}: {0}", e.Message,
                                    messageType.ToString("X")),
                                DebugType.Exception, true, stackTrace: e.StackTrace);
                            if (isReceiving)
                            {
                                StartReconnect();
                            }

                            break;
                        }


                        if (messageType >= typesList.ClientEnvelopesArray.Length ||
                            typesList.ClientEnvelopesArray[messageType] == null)
                        {
                            Debugger.Log(
                                "Received message is not yet supported. messageType = 0x" + messageType.ToString("X"),
                                DebugType.NetworkInterface, sendToServer: true, writeToUnityConsole: true);
                            break;
                        }

                        Debugger.Log(
                            string.Format("{0}(0x{1}) received", ((ClientMessageType) messageType),
                                messageType.ToString("X")), DebugType.NetworkInterface);
                        try
                        {
                            NetworkHandler.Receive(typesList.ClientEnvelopesArray[messageType].Create(cPacket));
                        }
                        catch (Exception e)
                        {
                            Debugger.Log(
                                string.Format(
                                    "Received message does not correspond to protocol used. Type of received message ain\'t equals to that of decoded.messageType = 0x{0}, trying to decode as {1}",
                                    messageType,
                                    typesList.ClientPacketTypesArray[messageType]),
                                DebugType.NetworkInterface, sendToServer: true, writeToUnityConsole: true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debugger.Log("Unhandled exception in tcp network interface. Details: " + e, writeToUnityConsole: true);
            }
            finally
            {
                ThreadManager.UnRegisterThread();
            }
        }

        private void StartReconnect()
        {
            //Hard-reset
            IsConnected = false;
            isReceiving = false;
            receiveEvent.Reset();
            reconnectEvent.Reset();

            OctClient.Reset();
        }

        public void OnReconnect()
        {
            IsConnected = true;
            CurrentStreamState = StreamState.Connected;
        }

        private void SendMessage(object message, ServerMessageType typePrefix, Type type)
        {
            if ((CurrentStreamState & (StreamState.Connected | StreamState.Reconnected)) == 0)
            {
                throw new Exception("Not connected to server");
            }
            networkStream.WriteByte((byte) typePrefix);
            serializer.SerializeWithLengthPrefix(networkStream, message, type, PrefixStyle.Fixed32BigEndian, 0);

            //            if (typePrefix != ServerMessageType.SDEBUGMESSAGE)
            //            {
            Debugger.Log(string.Format("Sending {0} (0x{1})", typePrefix, ((int) typePrefix).ToString("X")),
                DebugType.NetworkInterface);
            //            }
            networkStream.Flush();
        }
    }
}