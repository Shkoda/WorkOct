using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Assets.Src.Threading
{
    public static class ThreadManager
    {
        private static readonly Dictionary<int, Thread> Threads = new Dictionary<int, Thread>();

        private static readonly Dictionary<int, string> Descriptions = new Dictionary<int, string>();

        private static volatile int _threadCount = 1;

        private static volatile bool _isAborting;

        //        public static int MainThreadId;

        public static int ThreadCount
        {
            get { return _threadCount; }
        }

        public static bool IsRunningInMainThread
        {
            get { return Thread.CurrentThread.ManagedThreadId == MainThreadId; }
        }

        public static int MainThreadId { get; private set; }

        public static void RegisterThread(string descr)
        {
            try
            {
                int currentThreadId = Thread.CurrentThread.ManagedThreadId;

                lock (Threads)
                {
                    if (Threads.ContainsKey(currentThreadId))
                    {
                        return;
                    }

                    Threads.Add(currentThreadId, Thread.CurrentThread);
                }

                lock (Descriptions)
                {
                    Descriptions.Add(currentThreadId, descr);
                }

                Interlocked.Increment(ref _threadCount);

                Debugger.Log(String.Format("Registered thread {0}: {1}", currentThreadId, descr), DebugType.Threading);
            }
            catch (Exception e)
            {
                Debugger.Log(
                    String.Format(
                        "Error in ThreadManager while adding {0}. {1}. " +
                        "State of ThreadManager is not guaranted to represent actual information about threading anymore.",
                        descr,
                        e.Message));
            }
        }

        public static void UnRegisterThread()
        {
            try
            {
                if (!_isAborting)
                {
                    int currentThreadId = Thread.CurrentThread.ManagedThreadId;

                    lock (Descriptions)
                    {
                        Debugger.Log(
                            String.Format(
                                "Unregistering thread {0}: {1}",
                                currentThreadId,
                                Descriptions[currentThreadId]),
                            DebugType.Threading);
                        Descriptions.Remove(currentThreadId);
                    }

                    lock (Threads)
                    {
                        Threads.Remove(currentThreadId);
                    }

                    Interlocked.Decrement(ref _threadCount);
                }
                else
                {
                    int currentThreadId = Thread.CurrentThread.ManagedThreadId;
                    Debugger.Log(
                        String.Format(
                            "Unregistering (aborted) thread {0} (no description available on abort)",
                            currentThreadId),
                        DebugType.Threading);
                }
            }
            catch (Exception e)
            {
                Debugger.Log(
                    String.Format(
                        "Error in ThreadManager while removing. {0}. State of ThreadManager is not guaranted to represent actual information about threading anymore.",
                        e.Message));
                throw;
            }
        }

        public static void OutputRunningThreads()
        {
            var sb = new StringBuilder();
            int count = 0;
            lock (Descriptions)
            {
                count = Descriptions.Count;
                foreach (var description in Descriptions)
                {
                    sb.Append(description + ", ");
                }
            }
            Debugger.Log(String.Format("Running threads ({1}): {0}", sb, count), writeToUnityConsole: true);
        }

        public static void AbortAll()
        {
            _isAborting = true;
            lock (Threads)
            {
                foreach (var thread in Threads)
                {
                    //No fooling around
                    if (thread.Key != MainThreadId)
                    {
                        thread.Value.Abort();
                    }
                }
            }
            _isAborting = false;
        }

        public static void SetMainThread()
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}