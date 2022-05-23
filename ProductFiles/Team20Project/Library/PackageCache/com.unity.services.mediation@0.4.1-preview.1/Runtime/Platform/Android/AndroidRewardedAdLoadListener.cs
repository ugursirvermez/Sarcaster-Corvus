#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidRewardedAdLoadListener : AndroidJavaProxy, IAndroidRewardedLoadListener
    {
        IAndroidRewardedLoadListener m_Listener;
        public AndroidRewardedAdLoadListener(IAndroidRewardedLoadListener listener) : base("com.unity3d.mediation.IRewardedAdLoadListener")
        {
            m_Listener = listener;
        }

        public void onRewardedLoaded(AndroidJavaObject rewardedAd)
        {
            ThreadUtil.Post(state => m_Listener.onRewardedLoaded(rewardedAd));
        }

        public void onRewardedFailedLoad(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg)
        {
            ThreadUtil.Post(state => m_Listener.onRewardedFailedLoad(rewardedAd, error, msg));
        }
    }
}
#endif
