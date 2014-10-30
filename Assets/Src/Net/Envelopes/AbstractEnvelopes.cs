using Assets.Src.Net.Handler;

namespace WorkOct.Net.Envelopes
{
    public abstract class ClientEnvelope
    {
        public object Packet { get; set; }
        public abstract ClientMessageType PacketType { get; }

        /// <summary>
        /// Define behaviour of packet in this method.
        /// </summary>
        public abstract void Handle();

        /// <summary>
        /// Create envelope from packet coming from network.
        /// Every overriden Create method should return object which runtime type is of it's containing class.
        /// </summary>
        /// <param name="packet">data from network, which should be put into envelope for later handling</param>
        /// <returns></returns>
        public abstract ClientEnvelope Create(object packet);
    }

    public abstract class ServerEnvelope
    {
        public object Packet { get; set; }
        public abstract ServerMessageType PacketType { get; }

        public virtual bool IsPollRequest { get { return false; } }

        /// <summary>
        /// Send the packet over to NetworkHandler
        /// </summary>
        public void Send()
        {
            Debugger.Log(string.Format(" << Enqueuing Http envelope: {0}", GetType()));
        
            NetworkHandler.Send(this);
        }
    }
}