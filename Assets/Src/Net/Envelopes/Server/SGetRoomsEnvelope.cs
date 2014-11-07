using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;
using WorkOct.Protocol;

namespace Assets.Src.Net.Envelopes.Server
{
    class SGetRoomsEnvelope: ServerEnvelope
    {
        public override ServerMessageType PacketType
        {
            get { return ServerMessageType.SGETROOMS; }
        }

        public SGetRoomsEnvelope()
        {
            Packet = new SGetRooms();
        }
    }
}
