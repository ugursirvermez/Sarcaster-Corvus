#if UNITY_IOS

using System.Reflection;
using Google;
using GooglePlayServices;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Unity.Services.Mediation.Build.Editor
{
    public static class IosDependencyUpdatePostBuild
    {
        //IOSResolver.BUILD_ORDER_INSTALL_PODS = 50 (private), so this step is 51
        [PostProcessBuild(51)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            UnityEngine.Debug.Log("[Mediation] Calling pod update on Unity dependencies");
            var method = typeof(IOSResolver).GetMethod("RunPodCommand", BindingFlags.Static | BindingFlags.NonPublic);
            var returnValue = method?.Invoke(obj: null, parameters: new object[] { "update", pathToBuiltProject, false });
            var result = (CommandLine.Result)returnValue;
            if (result != null) UnityEngine.Debug.Log($"result.message: {result.message}");
            UnityEngine.Debug.Log("[Mediation] Finished pod update on Unity dependencies");
        }
    }
}

#endif
