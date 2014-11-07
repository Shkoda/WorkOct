using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;
using WorkOct.Protocol;

namespace Assets.Src.Net.Envelopes.Client
{
    public class CNewPlayerEnvelope : ClientEnvelope
    {
        public static event Action<List<RoomInfo>> HandleRoomList;
        public static event Action<PlayerInfo> HandlePlayerInfo;


        public override ClientMessageType PacketType
        {
            get { return ClientMessageType.CNEWPLAYER; }
        }

        public override void Handle()
        {
            Debugger.Log(" >> CNewPlayer handler");
            var cNewPlayer = (CNewPlayer) Packet;
            HandlePlayerInfo(cNewPlayer.playerInfo);
            HandleRoomList(cNewPlayer.rooms);
        }

        public override ClientEnvelope Create(object packet)
        {
            var envelope = new CNewPlayerEnvelope {Packet = packet};
            return envelope;
        }
    }
}