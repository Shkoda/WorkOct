using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;
using WorkOct.Protocol;

namespace Assets.Src.Net.Envelopes.Server
{
    internal class SNewPlayerEnvelope : ServerEnvelope
    {
        public override ServerMessageType PacketType
        {
            get { return ServerMessageType.SNEWPLAYER; }
        }

        public SNewPlayerEnvelope(string login)
        {
            Packet = new SNewPlayer() {name = login};
        }
    }
}