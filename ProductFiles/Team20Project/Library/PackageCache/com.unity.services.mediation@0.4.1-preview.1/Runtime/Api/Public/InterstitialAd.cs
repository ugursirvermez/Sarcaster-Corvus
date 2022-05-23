using System;
using Unity.Services.Mediation.Platform;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Class to be instantiated in order to show an Interstitial Ad.
    /// </summary>
    public sealed class InterstitialAd : IInterstitialAd
    {
        /// <summary>
        /// Event to be triggered by the adapter when an Ad is loaded.
        /// </summary>
        public event EventHandler OnLoaded;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad fails to load.
        /// </summary>
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad is started.
        /// </summary>
        public event EventHandler OnShowed;

        /// <summary>
        /// Event to be triggered by the adapter when the user clicks on the Ad.
        /// </summary>
        public event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad is closed.
        /// </summary>
        public event EventHandler OnClosed;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad has an error.
        /// </summary>
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        /// <summary>
        /// Get the current state of the ad.
        /// </summary>
        public AdState AdState => m_InterstitialAdImpl.AdState;

        /// <summary>
        /// Get the ad unit id set during construction.
        /// </summary>
        public string AdUnitId => m_InterstitialAdImpl.AdUnitId;

        IInterstitialAd m_InterstitialAdImpl;
        /// <summary>
        /// Constructor for managing a specific Interstitial Ad.
        /// </summary>
        /// <param name="adUnitId">Unique Id for the Ad you want to show.</param>
        public InterstitialAd(string adUnitId)
        {
#if UNITY_EDITOR
            m_InterstitialAdImpl = new EditorInterstitialAd(adUnitId);
#elif UNITY_ANDROID
            m_InterstitialAdImpl = new AndroidInterstitialAd(adUnitId);
#elif UNITY_IOS
            m_InterstitialAdImpl = new IosInterstitialAd(adUnitId);
#else
            m_InterstitialAdImpl = new UnsupportedInterstitialAd(adUnitId);
#endif
            InitializeCallbacks();
        }

        internal InterstitialAd(IInterstitialAd interstitialAdImpl)
        {
            m_InterstitialAdImpl = interstitialAdImpl;
            InitializeCallbacks();
        }

        void InitializeCallbacks()
        {
            m_InterstitialAdImpl.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_InterstitialAdImpl.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_InterstitialAdImpl.OnShowed += (sender, args) => OnShowed?.Invoke(this, args);
            m_InterstitialAdImpl.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_InterstitialAdImpl.OnClosed += (sender, args) => OnClosed?.Invoke(this, args);
            m_InterstitialAdImpl.OnFailedShow += (sender, args) => OnFailedShow?.Invoke(this, args);
        }

        /// <summary>
        /// Method to tell the Mediation SDK to load an Ad.
        /// </summary>
        public void Load() => m_InterstitialAdImpl.Load();

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        public void Show() => m_InterstitialAdImpl.Show();

        /// <summary>
        /// Dispose and release internal resources.
        /// </summary>
        public void Dispose() => m_InterstitialAdImpl.Dispose();
    }
}
