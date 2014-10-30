using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Src.Threading
{

    public static class MainThread
    {
        private static readonly List<MainThreadObject> MainThreadObjects = new List<MainThreadObject>();

        private static bool _updateBreak;

        internal static void Update()
        {
            MainThreadObject obj;
            for (int index = 0; index < MainThreadObjects.Count; index++)
            {
                lock (MainThreadObjects)
                {
                    if (index < MainThreadObjects.Count)
                    {
                        obj = MainThreadObjects[index];
                    }
                    else
                    {
                        break;
                    }
                }

                obj.DecrementDeltaTime(Time.deltaTime);

                if (obj.CallBack != null && obj.DeltaTime <= 0)
                {
                    try
                    {
                        obj.CallBack(obj.Args);
                        obj.ResetTime();
                    }
                    finally
                    {
                        if (obj.Once)
                        {
                            lock (MainThreadObjects)
                            {
                                MainThreadObjects.Remove(obj);
                                index--;
                            }
                        }
                    }
                }
            }
        }

        public static void Clear()
        {
            lock (MainThreadObjects)
            {
                MainThreadObjects.Clear();
            }
        }

        public static void Add(Action<object[]> callBack, float deltaTime = 0, string tag = null, params object[] args)
        {
            if (callBack == null)
            {
                return;
            }
            int index = IndexOfCallBack(callBack);

            if (index == -1)
            {
                lock (MainThreadObjects)
                {
                    MainThreadObjects.Add(
                        new MainThreadObject { CallBack = callBack, Args = args, DeltaTime = deltaTime, Tag = tag });
                }
            }
        }

        public static void AddOnce(Action<object[]> callBack, float deltaTime = 0, string tag = null, params object[] args)
        {
            if (callBack == null)
            {
                return;
            }

            int index = IndexOfCallBack(callBack);

            //            if (index == -1)
            //            {
            lock (MainThreadObjects)
            {
                MainThreadObjects.Add(
                    new MainThreadObject { CallBack = callBack, Args = args, Once = true, DeltaTime = deltaTime, Tag = tag });
            }
            //            }
        }

        public static void AddOnce(Action callBack, float deltaTime = 0, string tag = null)
        {
            if (callBack == null)
            {
                return;
            }

            var wrappedCallback = new Action<object[]>(objects => callBack());

            int index = IndexOfCallBack(wrappedCallback);

            //            if (index == -1)
            //            {
            lock (MainThreadObjects)
            {
                MainThreadObjects.Add(
                    new MainThreadObject { CallBack = wrappedCallback, Args = null, Once = true, DeltaTime = deltaTime, Tag = tag });
            }
            //            }
        }

        public static void Remove(Action<object[]> callBack)
        {
            //Make sure that index of callback is not changed during remove
            lock (MainThreadObjects)
            {
                int index = IndexOfCallBack(callBack);

                if (index != -1)
                {
                    MainThreadObjects.RemoveAt(index);
                }
            }
        }

        public static void Remove(string tag)
        {
            //Make sure that index of callback is not changed during remove
            lock (MainThreadObjects)
            {
                int index = IndexOfCallBack(tag);

                if (index != -1)
                {
                    MainThreadObjects.RemoveAt(index);
                }
            }
        }

        private static int IndexOfCallBack(Action<object[]> callBack)
        {
            lock (MainThreadObjects)
            {
                for (int i = 0; i < MainThreadObjects.Count; i++)
                {
                    if (MainThreadObjects[i].CallBack.Equals(callBack))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private static int IndexOfCallBack(string tag)
        {
            lock (MainThreadObjects)
            {
                for (int i = 0; i < MainThreadObjects.Count; i++)
                {
                    if (MainThreadObjects[i].Tag.Equals(tag))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static void Call(params object[] methods)
        {
            try
            {
                foreach (Action method in methods)
                {
                    try
                    {
                        method();
                    }
                    catch (Exception e)
                    {

                        Debug.LogException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
