using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net;
using WorkOct.Net.Envelopes;
using WorkOct.Protocol;

namespace Assets.Src.Net.Envelopes.Client
{
    public class CGetRoomsEnvelope : ClientEnvelope
    {
        
        public static event Action<List<RoomInfo>> HandleRoomList;
        public override ClientMessageType PacketType
        {
            get { return ClientMessageType.CGETROOMS; }
        }

        public override void Handle()
        {
            Debugger.Log("CGetRoomsEnvelope handler");
            var cGetRooms = (CGetRooms) Packet;
            HandleRoomList(cGetRooms.rooms);
        }

        public override ClientEnvelope Create(object packet)
        {
            var envelope = new CGetRoomsEnvelope {Packet = packet};
            return envelope;
        }

    }
}