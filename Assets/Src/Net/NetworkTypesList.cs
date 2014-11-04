using System;
using WorkOct.Net.Envelopes;
using Assets.Src.Net.Envelopes.Client;
using Assets.Src.Net.Envelopes.Server;
using WorkOct.Protocol;


namespace WorkOct.Net
{
    [Flags]
    public enum StreamState
    {
        NotOpened = 1,
        Connected = 0x2,
        Reconnecting = 0x4,
        Reconnected = 0x8,
        Closed = 0x10
    }

    public enum ClientMessageType
    {
        CPING = 0x01,


        //--------------//
        CNEWPLAYER = 0x10,


        //--------------//
        CGETPLAYERS = 0x20,
        CGETROOMS = 0x21,
        CCREATEROOM = 0x22,
        CJOINROOM = 0x23,
        CLEAVEROOM = 0x24,


        //--------------//
        CFIELD = 0x30,
    }


    public enum ServerMessageType
    {
        SPING = 0x01,


        //--------------//
        SNEWPLAYER = 0x10,


        //--------------//
        SGETPLAYERS = 0x20,
        SGETROOMS = 0x21,
        SCREATEROOM = 0x22,
        SJOINROOM = 0x23,
        SLEAVEROOM = 0x24,


        //--------------//
        SFIELD = 0x30,
    }

    public class NetworkTypesList
    {
        public readonly Type[] ClientPacketTypesArray =
            new[]
            {
                null, //0x00
                typeof (CPing), //0x01
                null, //0x02
                null, //0x03
                null, //0x04
                null, //0x05
                null, //0x06
                null, //0x07
                null, //0x08
                null, //0x09
                null, //0x0A
                null, //0x0B
                null, //0x0C
                null, //0x0D
                null, //0x0E
                null, //0x0F
                typeof (CNewPlayer), //0x10
                null, //0x11
                null, //0x12
                null, //0x13
                null, //0x14
                null, //0x15
                null, //0x16
                null, //0x17
                null, //0x18
                null, //0x19
                null, //0x1A
                null, //0x1B
                null, //0x1C
                null, //0x1D
                null, //0x1E
                null, //0x1F
                typeof (CGetPlayers), //0x20
                typeof (CGetRooms), //0x21
                typeof (CCreateRoom), //0x22
                typeof (CJoinRoom), //0x23
                typeof (CLeaveRoom), //0x24
                null, //0x25
                null, //0x26
                null, //0x27
                null, //0x28
                null, //0x29
                null, //0x2A
                null, //0x2B
                null, //0x2C
                null, //0x2D
                null, //0x2E
                null, //0x2F
                typeof (CField), //0x30
            };

        public readonly Type[] ServerPacketTypesArray =
            new[]
            {
                null, //0x00
                typeof (SPing), //0x01
                null, //0x02
                null, //0x03
                null, //0x04
                null, //0x05
                null, //0x06
                null, //0x07
                null, //0x08
                null, //0x09
                null, //0x0A
                null, //0x0B
                null, //0x0C
                null, //0x0D
                null, //0x0E
                null, //0x0F
                typeof (SNewPlayer), //0x10
                null, //0x11
                null, //0x12
                null, //0x13
                null, //0x14
                null, //0x15
                null, //0x16
                null, //0x17
                null, //0x18
                null, //0x19
                null, //0x1A
                null, //0x1B
                null, //0x1C
                null, //0x1D
                null, //0x1E
                null, //0x1F
                typeof (SGetPlayers), //0x20
                typeof (SGetRooms), //0x21
                typeof (SCreateRoom), //0x22
                typeof (SJoinRoom), //0x23
                typeof (SLeaveRoom), //0x24
                null, //0x25
                null, //0x26
                null, //0x27
                null, //0x28
                null, //0x29
                null, //0x2A
                null, //0x2B
                null, //0x2C
                null, //0x2D
                null, //0x2E
                null, //0x2F
                typeof (SField), //0x30
            };

        public readonly ClientEnvelope[] ClientEnvelopesArray =
            new ClientEnvelope[]
            {
                null,
                new CPingEnvelope(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new CNewPlayerEnvelope(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new CGetPlayersEnvelope(),
                new CGetRoomsEnvelope(),
                new CCreateRoomEnvelope(),
                new CJoinRoomEnvelope(),
                new CLeaveRoomEnvelope(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new CFieldEnvelope(),
            };
    }
}