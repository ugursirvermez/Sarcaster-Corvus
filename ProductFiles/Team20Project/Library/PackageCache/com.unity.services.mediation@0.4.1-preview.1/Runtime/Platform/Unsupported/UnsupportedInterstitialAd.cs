#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
using System;

namespace Unity.Services.Mediation.Platform
{
    class UnsupportedInterstitialAd : IInterstitialAd
    {
        public UnsupportedInterstitialAd(string adUnitId) {}

        public event EventHandler OnLoaded;

        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        public event EventHandler OnShowed;

        public event EventHandler OnClicked;

        public event EventHandler OnClosed;

        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        public AdState AdState => AdState.Unloaded;

        public string AdUnitId { get; }

        public void Load() {}

        public void Show() {}

        public void Dispose() {}
    }
}
#endif
