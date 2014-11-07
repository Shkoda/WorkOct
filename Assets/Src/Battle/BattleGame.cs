using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Src.Net.Envelopes.Client;
using WorkOct.Protocol;

namespace Assets.Src.Battle
{
    public class BattleGame
    {
        public RoomManager RoomManager { get; private set; }
        private PlayerInfo playerInfo;

        public BattleGame()
        {
            RoomManager = new RoomManager();

            CNewPlayerEnvelope.HandlePlayerInfo += SetPlayerInfo;
        }

        public void SetPlayerInfo(PlayerInfo otherInfo)
        {
            this.playerInfo = otherInfo;
        }
    }
}