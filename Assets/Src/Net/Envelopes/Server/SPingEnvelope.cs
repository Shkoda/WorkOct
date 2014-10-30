using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;
using WorkOct.Protocol;

namespace Assets.Src.Net.Envelopes.Server
{
    class SPingEnvelope: ServerEnvelope
    {
        public override ServerMessageType PacketType
        {
            get
            {
                return ServerMessageType.SPING;
            }
        }

        public SPingEnvelope()
        {
            Packet = new SPing() { greeting = "hello my darling"};
        }

        public SPingEnvelope(string value)
        {
            Packet = new SPing() { greeting = value };
        }
    }
}
