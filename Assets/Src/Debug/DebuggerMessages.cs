partial class Debugger
{
    #region Nested type: Message

    private class Message
    {
        protected int hash;

        public Message(string msg, DebugType debugType, float height, float top)
        {
            this.msg = msg;
            count = 1;
            DebugType = debugType;
            hash = msg.GetHashCode();
            Height = height;
            Top = top;
        }

        public DebugType DebugType { get; protected set; }
        public string msg { get; protected set; }
        public int count { get; set; }
        public float Height { get; set; }
        public float Top { get; set; }

        public override string ToString()
        {
            if (count == 1)
            {
                return DebugType == DebugType.Main ? msg : (DebugType + ": " + msg);
            }
            else
            {
                return count + "x " + (DebugType == DebugType.Main ? msg : (DebugType + ": " + msg));
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Message))
            {
                return false;
            }

            var other = obj as Message;
            if (hash.Equals(other.hash))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return hash;
        }
    }

    #endregion

    #region Nested type: WatchMessage

    private class WatchMessage : Message
    {
        public WatchMessage(int id, string msg, DebugType debugType, float height, float top)
            : base(msg, debugType, height, top)
        {
            this.id = id;
            hash = id.GetHashCode();
        }

        public int id { get; private set; }

        public override string ToString()
        {
            return DebugType == DebugType.Main ? msg : (DebugType + ": " + msg);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WatchMessage))
            {
                return false;
            }

            var other = obj as WatchMessage;
            if (hash.Equals(other.hash))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void UpdateMessage(string newMessage)
        {
            msg = newMessage;
        }
    }

    #endregion

    #region Nested type: WatchMessage

    private class ConsoleMessage : Message
    {
        public bool IsResponse { get; set; }

        public ConsoleMessage(string msg, bool isResponse, float height, float top)
            : base(msg, DebugType.Console, height, top)
        {
            IsResponse = isResponse;
        }

        public override string ToString()
        {
            return IsResponse ? msg : "> " + msg;
        }
    }

    #endregion
}