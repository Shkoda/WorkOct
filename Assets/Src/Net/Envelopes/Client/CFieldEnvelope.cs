using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;

namespace Assets.Src.Net.Envelopes.Client
{
    public class CFieldEnvelope : ClientEnvelope
    {
        public override ClientMessageType PacketType
        {
            get
            {
                return ClientMessageType.CFIELD;
            }
        }

        public override void Handle()
        {
            Debugger.Log("CFieldEnvelope handler");
//            var cEnterGame = (CEnterGame)Packet;
//            PlatformClient.CurrentGame.CreateWorld(cEnterGame);
        }

        public override ClientEnvelope Create(object packet)
        {
            var envelope = new CFieldEnvelope { Packet = packet };
            return envelope;
        }
    }
}
