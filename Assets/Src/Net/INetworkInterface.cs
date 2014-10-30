using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkOct.Net.Envelopes;

namespace Assets.Src.Net
{
    public interface INetworkInterface
    {
        event Action LostConnection;

        event Action ServerError;

        void StopReceiving(bool wait);

        void SendGoodbye();

        bool SendEnvelope(ServerEnvelope envelope);

        void Connect(string address, int port, bool secure);

        void SetSessionKey(ulong sessionKey);
    }
}