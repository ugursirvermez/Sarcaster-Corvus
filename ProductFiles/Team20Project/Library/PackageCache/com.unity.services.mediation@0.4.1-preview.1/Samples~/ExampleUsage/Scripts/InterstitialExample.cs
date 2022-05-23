using System;
using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;

namespace Unity.Services.Mediation.Samples
{
    /// <summary>
    /// Sample Implementation of Unity Mediation
    /// </summary>
    public class InterstitialExample : MonoBehaviour
    {
        [Header("Ad Unit Ids"), Tooltip("Ad Unit Ids for each platform that represent Mediation waterfalls.")]
        public string androidAdUnitId;
        [Tooltip("Ad Unit Ids for each platform that represent Mediation waterfalls.")]
        public string iosAdUnitId;

        IInterstitialAd m_InterstitialAd;

        async void Start()
        {
            try
            {
                Debug.Log("Initializing...");
                await UnityServices.InitializeAsync();
                Debug.Log("Initialized!");
                InitializationComplete();
            }
            catch (Exception e)
            {
                InitializationFailed(e);
            }
        }

        public void ShowInterstitial()
        {
            if (m_InterstitialAd.AdState == AdState.Loaded)
            {
                m_InterstitialAd.Show();
            }
        }

        void LoadAd()
        {
            m_InterstitialAd.Load();
        }

        void InitializationComplete()
        {
            // Impression Event
            MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
            
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    m_InterstitialAd = MediationService.Instance.CreateInterstitialAd(androidAdUnitId);
                    break;

                case RuntimePlatform.IPhonePlayer:
                    m_InterstitialAd = MediationService.Instance.CreateInterstitialAd(iosAdUnitId);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    m_InterstitialAd = MediationService.Instance.CreateInterstitialAd(!string.IsNullOrEmpty(androidAdUnitId) ? androidAdUnitId : iosAdUnitId);
                    break;
                default:
                    Debug.LogWarning("Mediation service is not available for this platform:" + Application.platform);
                    return;
            }

            // Load Events
            m_InterstitialAd.OnLoaded += AdLoaded;
            m_InterstitialAd.OnFailedLoad += AdFailedLoad;

            // Show Events
            m_InterstitialAd.OnClosed += AdClosed;
            m_InterstitialAd.OnShowed += AdShown;
            m_InterstitialAd.OnFailedShow += AdFailedShow;

            Debug.Log("Initialized On Start! Loading Ad...");
            LoadAd();
        }

        void InitializationFailed(Exception error)
        {
            SdkInitializationError initializationError = SdkInitializationError.Unknown;
            if (error is InitializeFailedException initializeFailedException)
            {
                initializationError = initializeFailedException.initializationError;
            }
            Debug.Log($"Initialization Failed: {initializationError}:{error.Message}");
        }

        void AdLoaded(object sender, EventArgs args)
        {
            Debug.Log("Loaded Interstitial!");
        }

        void AdFailedLoad(object sender, LoadErrorEventArgs args)
        {
            Debug.Log("Interstitial Load Failure");
            Debug.Log(args.Message);
        }

        void AdClosed(object sender, EventArgs args)
        {
            Debug.Log("Interstitial Closed! Loading Ad...");
            LoadAd();
        }

        void AdShown(object sender, EventArgs e)
        {
            Debug.Log("Interstitial Shown!");
        }

        void AdFailedShow(object sender, ShowErrorEventArgs args)
        {
            Debug.Log($"Interstitial failed to show : {args.Message}");
        }

        void ImpressionEvent(object sender, ImpressionEventArgs args)
        {
            var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
            Debug.Log($"Impression event from ad unit id {args.AdUnitId} : {impressionData}");
        }
    }
}
