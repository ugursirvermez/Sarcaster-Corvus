using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEngine.AdaptivePerformance.Samsung.Android
{
    /// <summary>
    /// Provider Settings for Samsung Android provider, which controls the runtime asset instance that stores the settings.
    /// </summary>
    [System.Serializable]
    [AdaptivePerformanceConfigurationData("Samsung (Android)", SamsungAndroidProviderConstants.k_SettingsKey)]
    public class SamsungAndroidProviderSettings : IAdaptivePerformanceSettings
    {
        [SerializeField, Tooltip("Enable Logging in Devmode")]
        bool m_SamsungProviderLogging = false;

        /// <summary>
        ///  Control debug logging of the Samsung provider.
        ///  This setting only affects development builds. All logging is disabled in release builds.
        ///  You can also control the global logging setting after startup by using <see cref="IDevelopmentSettings.Logging"/>.
        ///  Logging is disabled by default.
        /// </summary>
        /// <value>Set this to true to enable debug logging, or false to disable it (default: false).</value>
        public bool samsungProviderLogging
        {
            get { return m_SamsungProviderLogging; }
            set { m_SamsungProviderLogging = value; }
        }

        [SerializeField, Tooltip("Allow High-Speed Variable Refresh Rate. It is required if you want to use variable refresh rates higher than 60hz. Can increase device temperature when activated.")]
        bool m_HighSpeedVRR = false;

        /// <summary>
        ///  Use High-Speed Variable Refresh Rate to allow refresh rates higher than 60 fps set via VRR APIs.
        ///  This is required if you want to use variable refresh rates higher than 60hz.
        ///  Can increase device temperature when activated.
        ///  This setting only has an effect if a device supports Variable Refresh Rate.
        ///  Unity does not set High-Speed Variable Refresh Rate automatically by default.
        /// </summary>
        /// <value>Set this to true to allow High-Speed Variable Refresh Rate, or false to disable it (default: false).</value>
        public bool highSpeedVRR
        {
            get { return m_HighSpeedVRR; }
            set { m_HighSpeedVRR = value; }
        }

        [SerializeField, Tooltip("Enable Automatic Variable Refresh Rate. Only enabled if VRR is supported on the target device.")]
        bool m_AutomaticVRR = true;

        /// <summary>
        ///  Use automatic Variable Refresh Rate to set refresh rate automatically based on the timing of CPU, GPU, device thermal state, and target framerate.
        ///  This setting only affects the refresh rate if the device supports Variable Refresh Rate.
        ///  Unity sets Variable Refresh Rate automatically by default.
        /// </summary>
        /// <value>`Set this to true to enable Automatic Variable Refresh Rate, false to disable it (default: true if device supports Variable Refresh Rate).</value>
        public bool automaticVRR
        {
            get { return m_AutomaticVRR; }
            set { m_AutomaticVRR = value; }
        }

        /// <summary>Static instance that holds the runtime asset instance Unity creates during the build process.</summary>
#if !UNITY_EDITOR
        public static SamsungAndroidProviderSettings s_RuntimeInstance = null;
#endif
        void Awake()
        {
#if !UNITY_EDITOR
            s_RuntimeInstance = this;
#endif
        }

        /// <summary>
        /// Returns Android Provider Settings which are used by Adaptive Performance to apply Provider Settings.
        /// </summary>
        /// <returns>Android Provider Settings</returns>
        public static SamsungAndroidProviderSettings GetSettings()
        {
            SamsungAndroidProviderSettings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<SamsungAndroidProviderSettings>(SamsungAndroidProviderConstants.k_SettingsKey, out settings);
#else
            settings = s_RuntimeInstance;
#endif
            return settings;
        }
    }
}
