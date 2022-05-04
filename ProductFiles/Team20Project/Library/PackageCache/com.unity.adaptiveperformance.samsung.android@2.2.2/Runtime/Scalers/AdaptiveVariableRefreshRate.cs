using System;
using UnityEngine.AdaptivePerformance.Samsung.Android;

namespace UnityEngine.AdaptivePerformance
{
    /// <summary>
    /// A scaler used by <see cref="AdaptivePerformanceIndexer"/> to adjust the rendering rate using <see cref="VariableRefreshRate"/>.
    /// </summary>
    public class AdaptiveVariableRefreshRate : AdaptiveFramerate
    {
        /// <summary>
        /// Returns the name of the scaler.
        /// </summary>
        public override string Name  => "Adaptive Variable Refresh Rate";

        bool m_AdaptiveVRREnabled = false;

        /// <summary>
        /// Returns true if this scaler is active, false otherwise.
        /// </summary>
        public override bool Enabled { get => m_AdaptiveVRREnabled; set => m_AdaptiveVRREnabled = value; }


        IVariableRefreshRate m_VRR;
        int m_CurrentRefreshRateIndex;
        int m_DefaultRefreshRateIndex;

        /// <summary>
        /// Override for Awake in the base class to set up for Variable Refresh Rate.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (m_Settings == null)
                return;

            m_VRR = VariableRefreshRate.Instance;

            if (m_VRR == null)
            {
                Enabled = false;
                return;
            }

            m_VRR.RefreshRateChanged += RefreshRateChanged;
            m_CurrentRefreshRateIndex = Array.IndexOf(m_VRR.SupportedRefreshRates, m_VRR.CurrentRefreshRate);
        }

        /// <summary>
        /// Callback when scaler gets disabled and removed from indexer.
        /// </summary>
        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (m_VRR == null)
                return;

            m_VRR.SetRefreshRateByIndex(m_DefaultRefreshRateIndex);
        }

        /// <summary>
        /// Callback when scaler gets enabled and added to the indexer.
        /// </summary>
        protected override void OnEnabled()
        {
            base.OnEnabled();
            if (m_VRR == null)
                return;

            m_DefaultRefreshRateIndex = Array.IndexOf(m_VRR.SupportedRefreshRates, m_VRR.CurrentRefreshRate);
        }

        void OnDestroy()
        {
            if (m_VRR == null)
                return;

            m_VRR.RefreshRateChanged -= RefreshRateChanged;
        }

        void RefreshRateChanged()
        {
            if (m_VRR == null)
                return;

            m_CurrentRefreshRateIndex = Array.IndexOf(m_VRR.SupportedRefreshRates, m_VRR.CurrentRefreshRate);
        }

        /// <summary>
        /// Callback for when the performance level is increased.
        /// </summary>
        protected override void OnLevelIncrease()
        {
            if (m_VRR == null)
                return;

            if (m_CurrentRefreshRateIndex > 0)
            {
                var rateIndex = m_CurrentRefreshRateIndex - 1;
                var fps = m_VRR.SupportedRefreshRates[rateIndex];

                if (fps < MinBound || fps > MaxBound)
                    return;
                if (m_VRR.SetRefreshRateByIndex(rateIndex))
                    m_CurrentRefreshRateIndex = rateIndex;
            }
        }

        /// <summary>
        /// Callback for when the performance level is decreased.
        /// </summary>
        protected override void OnLevelDecrease()
        {
            if (m_VRR == null)
                return;

            if (m_CurrentRefreshRateIndex < m_VRR.SupportedRefreshRates.Length - 1)
            {
                var rateIndex = m_CurrentRefreshRateIndex + 1;
                var fps = m_VRR.SupportedRefreshRates[rateIndex];

                if (fps < MinBound || fps > MaxBound)
                    return;
                if (m_VRR.SetRefreshRateByIndex(rateIndex))
                    m_CurrentRefreshRateIndex = rateIndex;
            }
        }
    }
}
