using System.Collections.Generic;
using UnityEngine.AdaptivePerformance.Samsung.Android;
using UnityEditor.AdaptivePerformance.Editor.Metadata;
using UnityEngine;

namespace UnityEditor.AdaptivePerformance.Samsung.Android.Editor
{
    internal class SamsungAndroidProviderMetadata : IAdaptivePerformancePackage
    {
        private class SamsungAndroidPackageMetadata : IAdaptivePerformancePackageMetadata
        {
            public string packageName => "Adaptive Performance Samsung Android";
            public string packageId => "com.unity.adaptiveperformance.samsung.android";
            public string settingsType => "UnityEngine.AdaptivePerformance.Samsung.Android.SamsungAndroidProviderSettings";
            public string licenseURL => "https://docs.unity3d.com/Packages/com.unity.adaptiveperformance.samsung.android@latest?subfolder=/license/LICENSE.html";
            public List<IAdaptivePerformanceLoaderMetadata> loaderMetadata => s_LoaderMetadata;

            private readonly static List<IAdaptivePerformanceLoaderMetadata> s_LoaderMetadata = new List<IAdaptivePerformanceLoaderMetadata>() { new SamsungLoaderMetadata() };
        }

        private class SamsungLoaderMetadata : IAdaptivePerformanceLoaderMetadata
        {
            public string loaderName => "Samsung Android Provider";
            public string loaderType => "UnityEngine.AdaptivePerformance.Samsung.Android.SamsungAndroidProviderLoader";
            public List<BuildTargetGroup> supportedBuildTargets => s_SupportedBuildTargets;

            private readonly static List<BuildTargetGroup> s_SupportedBuildTargets = new List<BuildTargetGroup>()
            {
                BuildTargetGroup.Android
            };
        }

        private static IAdaptivePerformancePackageMetadata s_Metadata = new SamsungAndroidPackageMetadata();
        public IAdaptivePerformancePackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            var settings = obj as SamsungAndroidProviderSettings;
            if (settings != null)
            {
                settings.logging = false;
                settings.statsLoggingFrequencyInFrames = 50;
                settings.automaticPerformanceMode = true;

                return true;
            }

            return false;
        }
    }
}
