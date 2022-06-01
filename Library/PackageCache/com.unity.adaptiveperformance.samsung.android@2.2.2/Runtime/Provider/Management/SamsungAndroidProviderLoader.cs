using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AdaptivePerformance;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AdaptivePerformance.Editor;
#endif
using System.Runtime.InteropServices;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance.Samsung.Android
{
    /// <summary>
    /// SamsungAndroidProviderLoader implements the loader for Adaptive Performance on Samsung devices running Android.
    /// </summary>
#if UNITY_EDITOR
    [AdaptivePerformanceSupportedBuildTargetAttribute(BuildTargetGroup.Android)]
#endif
    public class SamsungAndroidProviderLoader : AdaptivePerformanceLoaderHelper
    {
        static List<AdaptivePerformanceSubsystemDescriptor> s_SamsungGameSDKSubsystemDescriptors =
            new List<AdaptivePerformanceSubsystemDescriptor>();

        #if UNITY_ANDROID
        /// <summary>Returns the currently active Samsung Android Subsystem instance, if an instance exists.</summary>
        public SamsungGameSDKAdaptivePerformanceSubsystem samsungGameSDKSubsystem
        {
            get { return GetLoadedSubsystem<SamsungGameSDKAdaptivePerformanceSubsystem>(); }
        }
#endif
        /// <summary>
        /// Implementation of <see cref="AdaptivePerformanceLoader.GetDefaultSubsystem"/>.
        /// </summary>
        /// <returns>Returns the Samsung Android Subsystem, which is the loaded default subsystem. Because only one subsystem can be present at a time, Adaptive Performance always initializes the first subsystem and uses it as a default. You can change subsystem order in the Adaptive Performance Provider Settings.</returns>
        public override ISubsystem GetDefaultSubsystem()
        {
#if UNITY_ANDROID
            return samsungGameSDKSubsystem;
#else
            return null;
#endif
        }

        /// <summary>
        /// Implementation of <see cref="AdaptivePerformanceLoader.GetSettings"/>.
        /// </summary>
        /// <returns>Returns the Samsung Android settings.</returns>
        public override IAdaptivePerformanceSettings GetSettings()
        {
            return SamsungAndroidProviderSettings.GetSettings();
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Initialize"/>.</summary>
        /// <returns>True if successfully initialized the Samsung Android subsystem, false otherwise.</returns>
        public override bool Initialize()
        {
#if UNITY_ANDROID
            CreateSubsystem<AdaptivePerformanceSubsystemDescriptor, SamsungGameSDKAdaptivePerformanceSubsystem>(s_SamsungGameSDKSubsystemDescriptors, "SamsungGameSDK");
            if (samsungGameSDKSubsystem == null)
            {
                Debug.LogError("Unable to start the Samsung Android subsystem.");
            }

            return samsungGameSDKSubsystem != null;
#else
            return false;
#endif
        }

        /// <summary>Implementation of <see cref="AdaptivePerformanceLoader.Start"/>.</summary>
        /// <returns>True if successfully started the Samsung Android subsystem, false otherwise.</returns>
        public override bool Start()
        {
#if UNITY_ANDROID
            StartSubsystem<SamsungGameSDKAdaptivePerformanceSubsystem>();
            return true;
#else
            return false;
#endif
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Stop"/>.</summary>
        /// <returns>True if successfully stopped the Samsung Android subsystem, false otherwise.</returns>
        public override bool Stop()
        {
#if UNITY_ANDROID
            StopSubsystem<SamsungGameSDKAdaptivePerformanceSubsystem>();
            return true;
#else
            return false;
#endif
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Deinitialize"/>.</summary>
        /// <returns>True if successfully deinitialized the Samsung Android subsystem, false otherwise.</returns>
        public override bool Deinitialize()
        {
#if UNITY_ANDROID
            DestroySubsystem<SamsungGameSDKAdaptivePerformanceSubsystem>();
            return true;
#else
            return false;
#endif
        }
    }
}
