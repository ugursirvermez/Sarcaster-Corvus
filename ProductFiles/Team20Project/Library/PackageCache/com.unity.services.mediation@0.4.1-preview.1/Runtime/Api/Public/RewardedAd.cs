using System;
using Unity.Services.Mediation.Platform;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Class to be instantiated in order to show a Rewarded Ad.
    /// </summary>
    public sealed class RewardedAd : IRewardedAd
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
        /// Event to be triggered by the adapter when a Rewarded Ad is shown.
        /// </summary>
        public event EventHandler OnShowed;

        /// <summary>
        /// Event to be triggered by the adapter when the user clicks on the RewardedAd.
        /// </summary>
        public event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the RewardedAd is closed.
        /// </summary>
        public event EventHandler OnClosed;

        /// <summary>
        /// Event to be triggered by the adapter when the RewardedAd has an error.
        /// </summary>
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        /// <summary>
        /// Event to be triggered by the adapter when a reward needs to be issued.
        /// </summary>
        public event EventHandler<RewardEventArgs> OnUserRewarded;

        /// <summary>
        ///<value>Gets the state of the <c>RewardedAd</c>.</value>
        /// </summary>
        public AdState AdState => m_RewardedAdImpl.AdState;

        /// <summary>
        /// <value>Gets the id of the ad unit.</value>
        /// </summary>
        public string AdUnitId => m_RewardedAdImpl.AdUnitId;

        IRewardedAd m_RewardedAdImpl;

        /// <summary>
        /// Constructor for managing a specific Rewarded Ad.
        /// </summary>
        /// <param name="adUnitId">Unique Id for the Ad you want to show.</param>
        public RewardedAd(string adUnitId)
        {
#if UNITY_EDITOR
            m_RewardedAdImpl = new EditorRewardedAd(adUnitId);
#elif UNITY_ANDROID
            m_RewardedAdImpl = new AndroidRewardedAd(adUnitId);
#elif UNITY_IOS
            m_RewardedAdImpl = new IosRewardedAd(adUnitId);
#else
            m_RewardedAdImpl = new UnsupportedRewardedAd(adUnitId);
#endif
            InitializeCallbacks();
        }

        internal RewardedAd(IRewardedAd rewardedAdImpl)
        {
            m_RewardedAdImpl = rewardedAdImpl;
            InitializeCallbacks();
        }

        void InitializeCallbacks()
        {
            m_RewardedAdImpl.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_RewardedAdImpl.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_RewardedAdImpl.OnShowed += (sender, args) => OnShowed?.Invoke(this, args);
            m_RewardedAdImpl.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_RewardedAdImpl.OnClosed += (sender, args) => OnClosed?.Invoke(this, args);
            m_RewardedAdImpl.OnFailedShow += (sender, args) => OnFailedShow?.Invoke(this, args);
            m_RewardedAdImpl.OnUserRewarded += (sender, args) => OnUserRewarded?.Invoke(this, args);
        }

        /// <summary>
        /// Method to tell the Mediation SDK to load an Ad.
        /// </summary>
        public void Load() => m_RewardedAdImpl.Load();

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        public void Show(RewardedAdShowOptions showOptions = null) => m_RewardedAdImpl.Show(showOptions);

        /// <summary>
        /// Dispose and release internal resources.
        /// </summary>
        public void Dispose() => m_RewardedAdImpl.Dispose();
    }
}
