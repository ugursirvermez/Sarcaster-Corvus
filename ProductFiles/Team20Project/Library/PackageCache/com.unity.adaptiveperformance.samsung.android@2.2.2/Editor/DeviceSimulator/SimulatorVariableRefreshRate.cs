using UnityEngine;
using System;
using UnityEngine.Scripting;
using UnityEngine.AdaptivePerformance.Samsung.Android;
using System.Collections.Generic;

namespace UnityEditor.AdaptivePerformance.Simulator.Editor
{
    /// <summary>
    /// Describes the simulated Motion smoothness value in Android Display settings.
    /// </summary>
    public enum AndroidDisplayRefreshRate
    {
        /// <summary>
        /// Allow high speed refresh rates with a minimum of 60 Hz.
        /// </summary>
        HighSpeed,

        /// <summary>
        /// Only allow standard refresh rates. 60 Hz and below.
        /// </summary>
        Standard
    }

    /// <summary>
    /// Interface of the Samsung Variable Refresh Rate API for use with the Device Simulator.
    /// </summary>
    public interface IVariableRefreshRateSimulator : IVariableRefreshRate
    {
        /// <summary>
        /// Sets the simulated display refresh mode.
        /// </summary>
        /// <param name="rate">The display mode to set.</param>
        void SetAndroidDisplayRefreshRateMode(AndroidDisplayRefreshRate rate);
        /// <summary>
        /// Sets the simulated display refresh rate.
        /// </summary>
        /// <param name="targetRefreshRate">The display refresh rate to set.</param>
        void SetRefreshRate(int targetRefreshRate);
        /// <summary>
        /// Gets the simulated display refresh rate.
        /// </summary>
        /// <returns>The simulated display refresh rate.</returns>
        int GetCurrentRefreshRate();
        /// <summary>
        /// Sets the simulated Android Provider Settings.
        /// </summary>
        /// <param name="vrrSettings">The simulated Android Provider Settings</param>
        void SetSettings(VRRSettings vrrSettings);
        /// <summary>
        /// Gets the simulated Android Provider Settings.
        /// </summary>
        /// <returns>The simulated Android Provider Settings.</returns>
        VRRSettings GetSettings();
    }

    /// <summary>
    /// A wrapper around the VRR settings in the Samsung provider to be usable in the Simulator.
    /// </summary>
    public struct VRRSettings
    {
        /// <summary>
        /// highSpeedVRR set to true if high-speed VRR should be simulated and false otherwise.
        /// </summary>
        public bool highSpeedVRR;
        /// <summary>
        /// automaticVRR set to true if automatic VRR should be simulated and false otherwise.
        /// </summary>
        public bool automaticVRR;
    }

    internal class VRRManagerSimulator : IVariableRefreshRateSimulator
    {
        VRRSettings settings = new VRRSettings
        {
            highSpeedVRR = false,
            automaticVRR = true,
        };

        int m_RefreshRate = 60;
        bool m_SystemEvent = false;

        AutoVariableRefreshRate m_AutoVariableRefreshRate;
        AndroidDisplayRefreshRate m_DisplayRefreshRateMode = AndroidDisplayRefreshRate.HighSpeed;
        readonly int[] m_StandardRefreshRates = { 48, 60 };
        readonly int[] m_HighResolutionRefreshRates = { 60, 96, 120 };

        private int[] GetSupportedRefreshRates()
        {
            switch (m_DisplayRefreshRateMode)
            {
                case AndroidDisplayRefreshRate.HighSpeed:
                    return m_HighResolutionRefreshRates;
                case AndroidDisplayRefreshRate.Standard:
                    return m_StandardRefreshRates;
            }

            return null;
        }

        public void SetAndroidDisplayRefreshRateMode(AndroidDisplayRefreshRate rate)
        {
            m_DisplayRefreshRateMode = rate;
            OnRefreshRateChanged();
        }

        public void SetRefreshRate(int targetRefreshRate)
        {
            m_RefreshRate = targetRefreshRate;
            m_SystemEvent = true;
            OnRefreshRateChanged();
        }

        bool SetRefreshRateGameSDK(int targetRefreshRate)
        {
            if (m_RefreshRate == targetRefreshRate || m_SystemEvent)
                return false;

            m_RefreshRate = targetRefreshRate;
            return true;
        }

        public int GetCurrentRefreshRate()
        {
            return m_RefreshRate;
        }

        public void SetSettings(VRRSettings vrrSettings)
        {
            settings = vrrSettings;
        }

        public VRRSettings GetSettings()
        {
            return settings;
        }

        object m_RefreshRateChangedLock = new object();
        bool m_RefreshRateChanged;
        int[] m_SupportedRefreshRates = new int[0];
        int m_CurrentRefreshRate = -1;
        int m_LastSetRefreshRate = -1;

        private void UpdateRefreshRateInfo()
        {
            var supportedRefreshRates = GetSupportedRefreshRates();
            if (settings.highSpeedVRR)
            {
                m_SupportedRefreshRates = supportedRefreshRates;
            }
            else
            {
                List<int> shrunkSupportedRefreshRates = new List<int>();
                for (var i = 0; i < supportedRefreshRates.Length; ++i)
                {
                    if (supportedRefreshRates[i] <= 60)
                        shrunkSupportedRefreshRates.Add(supportedRefreshRates[i]);
                }
                m_SupportedRefreshRates = shrunkSupportedRefreshRates.ToArray();
            }

            m_CurrentRefreshRate = GetCurrentRefreshRate();
        }

        public VRRManagerSimulator()
        {
            SetDefaultVRR();
            UpdateRefreshRateInfo();
            m_AutoVariableRefreshRate = new AutoVariableRefreshRate(this);
        }

        // If HighSpeedVRR is not enabled we should not set over 60hz by default
        private void SetDefaultVRR()
        {
            if (settings.highSpeedVRR)
                return;

            var index = Array.IndexOf(m_SupportedRefreshRates, 60);

            if (index != -1)
            {
                SetRefreshRateByIndexInternal(index);
            }
        }

        public void Resume()
        {
            bool changed = false;

            var oldSupportedRefreshRates = m_SupportedRefreshRates;
            var oldRefreshRate = m_LastSetRefreshRate;

            UpdateRefreshRateInfo();

            if (m_CurrentRefreshRate != oldRefreshRate)
                changed = true;
            else if (oldSupportedRefreshRates != m_SupportedRefreshRates)
                changed = true;

            if (changed)
            {
                lock (m_RefreshRateChangedLock)
                {
                    m_RefreshRateChanged = true;
                }
            }
        }

        public void Update()
        {
            bool refreshRateChanged = false;
            lock (m_RefreshRateChangedLock)
            {
                refreshRateChanged = m_RefreshRateChanged;
                m_RefreshRateChanged = false;
            }

            if (refreshRateChanged)
            {
                UpdateRefreshRateInfo();

                var index = Array.IndexOf(m_SupportedRefreshRates, m_LastSetRefreshRate);

                if (index != -1)
                {
                    SetRefreshRateByIndexInternal(index);
                }
                else if (index == -1 && m_LastSetRefreshRate != -1)
                {
                    // Previous set refresh rate is not in available in the refreshrate list.
                    // Need to set 60Hz or lowest refresh rate possible.
                    // User sets 48Hz, but 48Hz is not on list anymore, because user changed Setting App - Display - Smooth option.
                    index = Array.IndexOf(m_SupportedRefreshRates, 60);

                    if (index != -1)
                        SetRefreshRateByIndexInternal(index);
                }
                RefreshRateChanged?.Invoke();
            }
            if (settings.automaticVRR && QualitySettings.vSyncCount == 0)
                m_AutoVariableRefreshRate.UpdateAutoVRR();
        }

        public int[] SupportedRefreshRates { get { return m_SupportedRefreshRates; } }
        public int CurrentRefreshRate { get { return m_CurrentRefreshRate; } }

        public bool SetRefreshRateByIndex(int index)
        {
            // Refreshrate potentially set by user
            settings.automaticVRR = false;
            // reset system event
            m_SystemEvent = false;
            return SetRefreshRateByIndexInternal(index);
        }

        private bool SetRefreshRateByIndexInternal(int index)
        {
            if (index >= 0 && index < SupportedRefreshRates.Length)
            {
                var refreshRateFromIndex = SupportedRefreshRates[index];
                if (Application.targetFrameRate > 0 && index > 0 && SupportedRefreshRates[--index] > Application.targetFrameRate)
                {
                    Debug.Log($"SetRefreshRateByIndex tries to set the refreshRateTarget {refreshRateFromIndex} much higher than the targetFrameRate {Application.targetFrameRate}, which is not recommended due to temperature increase and unused performance.");
                }
                if (!settings.highSpeedVRR)
                {
                    if (refreshRateFromIndex > 60)
                    {
                        Debug.Log($"High-Speed VRR is not enabled in the settings. Setting a refresh rate ({refreshRateFromIndex}Hz) over 60Hz is not permitted to prevent temperature-related issues.");
                        return false;
                    }
                }
                if (SetRefreshRateGameSDK(refreshRateFromIndex))
                {
                    m_CurrentRefreshRate = refreshRateFromIndex;
                    m_LastSetRefreshRate = refreshRateFromIndex;
                    return true;
                }
            }
            return false;
        }

        public event VariableRefreshRateEventHandler RefreshRateChanged;

        public void OnRefreshRateChanged()
        {
            lock (m_RefreshRateChangedLock)
            {
                m_RefreshRateChanged = true;
            }
        }
    }
    internal class AutoVariableRefreshRate
    {
        IVariableRefreshRateSimulator vrrManager;

        public AutoVariableRefreshRate(IVariableRefreshRateSimulator vrrManagerInstance)
        {
            vrrManager = vrrManagerInstance;
        }

        // Temperature checks of hardware are around 5sec and we don't need to check that often.
        float VrrUpdateTime = 1;
        int lastRefreshRateIndex = -1;

        public void UpdateAutoVRR()
        {
            VrrUpdateTime -= Time.unscaledDeltaTime;

            if (VrrUpdateTime <= 0)
            {
                VrrUpdateTime = 1;

                // targetFPS = 70 (in 48/60/96/120)-> vrr 96 never 120
                // targetFPS = 40 (in 48/60/96/120)-> vrr 60 never 96
                // targetFPS = 48/60/96/120 (in 48/60/96/120) -> vrr 48/60/96/12 never higher
                // targetFPS = 70 (in 48/60)-> 60
                var refreshRateIndex = vrrManager.SupportedRefreshRates.Length - 1;
                // we look if a targetFrameRate is set, even in vsync mode were target framerate is ignored. Otherwise we use maximum framerate
                if (Application.targetFrameRate > 0)
                {
                    for (int i = 0; i < vrrManager.SupportedRefreshRates.Length; ++i)
                    {
                        if (Application.targetFrameRate > vrrManager.SupportedRefreshRates[i])
                        {
                            continue;
                        }
                        else
                        {
                            refreshRateIndex = i;
                            break;
                        }
                    }
                }

                if (lastRefreshRateIndex != refreshRateIndex)
                {
                    lastRefreshRateIndex = refreshRateIndex;
                    vrrManager.SetRefreshRateByIndex(refreshRateIndex);
                    // automatic VRR gets disabled in SetRefreshRateByIndex and we want to ensure we still get updated.
                    var settings = vrrManager.GetSettings();
                    settings.automaticVRR = true;
                    vrrManager.SetSettings(settings);
                }
            }
        }
    }
}
