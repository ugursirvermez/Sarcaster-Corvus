#if DEVICE_SIMULATOR_ENABLED || UNITY_2021_1_OR_NEWER
using System;
using System.Collections.Generic;
#if UNITY_2021_1_OR_NEWER
using UnityEditor.DeviceSimulation;
#elif DEVICE_SIMULATOR_ENABLED
using Unity.DeviceSimulator;
#endif
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.AdaptivePerformance.Simulator.Editor;
using UnityEngine.AdaptivePerformance.Samsung.Android;

namespace UnityEditor.AdaptivePerformance.Samsung.Android.Editor
{
    public class AdaptivePerformanceVRRUIExtension :
#if UNITY_2021_1_OR_NEWER
        DeviceSimulatorPlugin
#elif DEVICE_SIMULATOR_ENABLED
        IDeviceSimulatorExtension
#endif
        , ISerializationCallbackReceiver
    {
#if UNITY_2021_1_OR_NEWER
        override public string title
#elif DEVICE_SIMULATOR_ENABLED
        public string extensionTitle
#endif
        { get { return "Adaptive Performance Samsung"; } }

        VisualElement m_ExtensionFoldout;
        Foldout m_SettingsFoldout;
        Foldout m_VrrFoldout;
        Foldout m_AndroidSystemFoldout;
        Toggle m_HighSpeedVRR;
        Toggle m_AutomaticVRR;
        Toggle m_VRRSupport;
        PopupField<string> m_DisplayRefreshRateModes;
        PopupField<string> m_DisplayRefreshRates;
        PopupField<string> m_SupportedRR;
        SimulatorAdaptivePerformanceSubsystem m_Subsystem;
        List<string> m_HighModes = new List<string> { "120", "96", "60" };
        List<string> m_StandardModes = new List<string> { "60", "48" };
        VRRManagerSimulator m_vrrManager = new VRRManagerSimulator();

        [SerializeField, HideInInspector]
        AdaptivePerformanceStates m_SerializationStates;

#if UNITY_2021_1_OR_NEWER
        override public VisualElement OnCreateUI()
        {
            m_ExtensionFoldout = new VisualElement();
#elif DEVICE_SIMULATOR_ENABLED
        public void OnExtendDeviceSimulator(VisualElement visualElement)
        {
            m_ExtensionFoldout = visualElement;
#endif
            VariableRefreshRate.Instance = m_vrrManager;

            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.unity.adaptiveperformance.samsung.android/Editor/DeviceSimulator/AdaptivePerformanceExtension.uxml");
            if (tree == null)
            {
                Label warningLabel = new Label("Simulator provider is not installed. Please install and enable the provider in Project Settings > Adaptive Performance > Standalone >  Providers. After installation, close and reopen the Device Simulator to take effect.");
                warningLabel.style.whiteSpace = WhiteSpace.Normal;
                m_ExtensionFoldout.Add(warningLabel);
#if UNITY_2021_1_OR_NEWER
                return m_ExtensionFoldout;
#else
                return;
#endif
            }

            m_ExtensionFoldout.Add(tree.CloneTree());

            m_SettingsFoldout = m_ExtensionFoldout.Q<Foldout>("vrr-settings");
            m_SettingsFoldout.value = m_SerializationStates.settingsFoldout;

            m_HighSpeedVRR = m_ExtensionFoldout.Q<Toggle>("highspeed-vrr");
            m_HighSpeedVRR.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                var settings = m_vrrManager.GetSettings();
                settings.highSpeedVRR = evt.newValue;
                m_vrrManager.SetSettings(settings);
            });

            m_AutomaticVRR = m_ExtensionFoldout.Q<Toggle>("automatic-vrr");
            m_AutomaticVRR.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                var settings = m_vrrManager.GetSettings();
                settings.automaticVRR = evt.newValue;
                m_vrrManager.SetSettings(settings);
            });

            m_VrrFoldout = m_ExtensionFoldout.Q<Foldout>("vrr");
            m_VrrFoldout.value = m_SerializationStates.vrrFoldout;
            m_AndroidSystemFoldout = m_ExtensionFoldout.Q<Foldout>("android-system");
            m_AndroidSystemFoldout.value = m_SerializationStates.vrrFoldout;

            m_VRRSupport = m_ExtensionFoldout.Q<Toggle>("vrr-support");
            m_VRRSupport.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.newValue)
                {
                    VariableRefreshRate.Instance = m_vrrManager;
                }
                else
                {
                    VariableRefreshRate.Instance = null;
                }
            });

            var rrChoices = new List<string> {"48", "60", "96", "120"};
            m_DisplayRefreshRates = new PopupField<string>("Display Refresh Rate", rrChoices, 2);
            m_DisplayRefreshRates.tooltip = "Select the Display Refresh Rate of the Android Settings to simulate.";
            m_AndroidSystemFoldout.Add(m_DisplayRefreshRates);

            m_DisplayRefreshRates.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                m_vrrManager.SetRefreshRate(int.Parse(evt.newValue));
            });

            var choices = new List<string> {"120", "60"};
            m_DisplayRefreshRateModes = new PopupField<string>("Display Mode", choices, 0);
            m_DisplayRefreshRateModes.tooltip = "Select motion smoothness mode of the Android Settings to simulate standard (60 Hz) or high refresh rate (120 Hz).";
            m_AndroidSystemFoldout.Add(m_DisplayRefreshRateModes);


            AddSupportedModesPopup(m_DisplayRefreshRateModes.index);

            m_DisplayRefreshRateModes.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (m_vrrManager == null)
                    return;

                m_VrrFoldout.Remove(m_SupportedRR);

                AddSupportedModesPopup(m_DisplayRefreshRateModes.index);

                m_vrrManager.SetAndroidDisplayRefreshRateMode((AndroidDisplayRefreshRate)m_DisplayRefreshRateModes.index);
            });

            EditorApplication.playModeStateChanged += LogPlayModeState;

#if UNITY_2021_1_OR_NEWER
            return m_ExtensionFoldout;
#endif
        }

        void AddSupportedModesPopup(int displaySettingIndex)
        {
            switch (m_DisplayRefreshRateModes.index)
            {
                case 0:
                    m_SupportedRR = new PopupField<string>("Refresh Rate", m_HighModes, 2);
                    break;
                case 1:
                    m_SupportedRR = new PopupField<string>("Refresh Rate", m_StandardModes, 1);
                    break;
            }
            m_SupportedRR.tooltip = "Select the Refresh Rate to set via the VRR API.";
            m_VrrFoldout.Add(m_SupportedRR);

            m_SupportedRR.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (VariableRefreshRate.Instance == null)
                    return;

                VariableRefreshRate.Instance.SetRefreshRateByIndex(m_SupportedRR.index);
            });
        }

        [System.Serializable]
        internal struct AdaptivePerformanceStates
        {
            public bool settingsFoldout;
            public bool vrrFoldout;
            public bool androidSystemFoldout;
        };

        public void OnBeforeSerialize()
        {
            if (m_SettingsFoldout == null)
                return;

            m_SerializationStates.settingsFoldout = m_SettingsFoldout.value;
            m_SerializationStates.vrrFoldout = m_VrrFoldout.value;
            m_SerializationStates.androidSystemFoldout = m_AndroidSystemFoldout.value;
        }

        public void OnAfterDeserialize() {}

        void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
                EditorApplication.update += Update;
            else
                EditorApplication.update -= Update;
        }

        void Update()
        {
            if (m_vrrManager == null || m_DisplayRefreshRates == null)
                return;

            m_vrrManager.Update();
            var currentRR = m_vrrManager.GetCurrentRefreshRate();

            if (int.Parse(m_DisplayRefreshRates.value) != currentRR)
                m_DisplayRefreshRates.SetValueWithoutNotify(currentRR.ToString());
        }
    }
}
#endif
