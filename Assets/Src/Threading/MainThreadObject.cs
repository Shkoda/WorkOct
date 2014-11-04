using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Src.Threading
{
    internal class MainThreadObject
    {
        private float _defaultDeltaTime;

        private float _deltaTime;

        private volatile bool _once;

        public Action<object[]> CallBack { get; set; }
        public object[] Args { get; set; }

        public string Tag { get; set; }

        public float DeltaTime
        {
            get { return this._deltaTime; }
            set
            {
                this._deltaTime = value;
                this._defaultDeltaTime = value;
            }
        }

        public bool Once
        {
            get { return this._once; }
            set { this._once = value; }
        }

        public void DecrementDeltaTime(float time)
        {
            this._deltaTime -= time;
        }

        public void ResetTime()
        {
            this._deltaTime = this._defaultDeltaTime;
        }
    }
}