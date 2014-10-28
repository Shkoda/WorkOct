using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;

namespace Assets.Src.Net.Envelopes.Client
{
   public class CPingEnvelope : ClientEnvelope
    {
        public override ClientMessageType PacketType
        {
            get
            {
                return ClientMessageType.CPING;
            }
        }

        public override void Handle()
        {
            Debugger.Log("CPing handler");
//            var cEnterGame = (CEnterGame)Packet;
//            PlatformClient.CurrentGame.CreateWorld(cEnterGame);
        }

        public override ClientEnvelope Create(object packet)
        {
            var envelope = new CPingEnvelope { Packet = packet };
            return envelope;
        }
    }
}
