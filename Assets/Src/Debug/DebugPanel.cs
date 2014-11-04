//using ItKpi.Net.Envelopes.Server;

using UnityEngine;

namespace Slots.Debug
{
    public class DebugPanel : MonoBehaviour
    {
        private const float ButtonsHeight = 40f;
        private const float ButtonsWidth = 100f;
        private bool panelOpened;

        public void OnGUI()
        {
            if (GUI.Button(new Rect(0, Screen.height - ButtonsHeight, ButtonsWidth, ButtonsHeight), "Controls"))
            {
                panelOpened = !panelOpened;
            }

            if (panelOpened)
            {
            }
        }
    }
}