using UnityEngine;

namespace Unity.Services.Core.Internal
{
    static class UnityServicesInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void CreateStaticInstance()
        {
            CoreRegistry.Instance = new CoreRegistry();
            UnityServices.Instance = new UnityServicesInternal(CoreRegistry.Instance);
            UnityServices.InstantiationCompletion?.TrySetResult(null);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnableServicesInitialization()
        {
            var instance = (UnityServicesInternal)UnityServices.Instance;
            instance.EnableInitialization();
        }
    }
}
