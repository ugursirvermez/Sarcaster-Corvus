using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Core
{
    static class MainThreadUtils
    {
        static int s_MainThreadId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void SetMainThreadId()
        {
            s_MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public static bool IsRunningOnMainThread
        {
            get { return Thread.CurrentThread.ManagedThreadId == s_MainThreadId; }
        }
    }
}
