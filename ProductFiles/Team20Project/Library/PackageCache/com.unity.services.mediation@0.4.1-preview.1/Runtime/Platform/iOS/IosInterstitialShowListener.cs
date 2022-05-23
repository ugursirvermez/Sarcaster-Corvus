#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosInterstitialShowListener : IosNativeObject
    {
        delegate void StartedCallback(IntPtr interstitialAd);
        delegate void ClickedCallback(IntPtr interstitialAd);
        delegate void FinishedCallback(IntPtr interstitialAd);
        delegate void FailedShowCallback(IntPtr interstitialAd, int error, string message);

        public IosInterstitialShowListener() : base(false)
        {
            NativePtr = InterstitialAdShowDelegateCreate(Started, Clicked, Finished, FailedShow);
        }

        public override void Dispose()
        {
            if (NativePtr == IntPtr.Zero) return;
            InterstitialAdShowDelegateDestroy(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdShowDelegateCreate")]
        static extern IntPtr InterstitialAdShowDelegateCreate(StartedCallback startedCallback, ClickedCallback clickedCallback,
            FinishedCallback finishedCallback, FailedShowCallback failedShowCallback);

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdShowDelegateDestroy")]
        static extern void InterstitialAdShowDelegateDestroy(IntPtr ptr);

        [MonoPInvokeCallback(typeof(StartedCallback))]
        static void Started(IntPtr ptr)
        {
            var interstitialAd = Get<IosInterstitialAd>(ptr);
            interstitialAd?.InvokeStartedEvent();
        }

        [MonoPInvokeCallback(typeof(ClickedCallback))]
        static void Clicked(IntPtr ptr)
        {
            var interstitialAd = Get<IosInterstitialAd>(ptr);
            interstitialAd?.InvokeClickedEvent();
        }

        [MonoPInvokeCallback(typeof(FinishedCallback))]
        static void Finished(IntPtr ptr)
        {
            var interstitialAd = Get<IosInterstitialAd>(ptr);
            interstitialAd?.InvokeFinishedEvent();
        }

        [MonoPInvokeCallback(typeof(FailedShowCallback))]
        static void FailedShow(IntPtr ptr, int error, string message)
        {
            var interstitialAd = Get<IosInterstitialAd>(ptr);
            interstitialAd?.InvokeFailedShowEvent(new ShowErrorEventArgs((ShowError)error, message));
        }
    }
}
#endif
