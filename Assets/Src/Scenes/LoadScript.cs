using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Src.Scenes
{
    class LoadScript:MonoBehaviour
    {
        public void LoadBattleScene()
        {
            Debugger.Log("LoadScript.LoadBattleScene()");
            Application.LoadLevel("BattleScene");
        }
    }
}
