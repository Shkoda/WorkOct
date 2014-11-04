using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Src.Net.Handler
{
    public enum NetworkState
    {
        ///Not connected to server
        NotConnected = 0,

        ///Socket connected, no data sent whatsoever
        ConnectionAcquired = 1,

        ///Starting packet send
        VersionSent = 2,

        ///Process of logging in to server started
        LoggingIn = 3,

        ///Already logged in to server
        LoggedIn = 4,

        ///In global game, main send cycle
        InGameAwaiting = 5,
    }
}