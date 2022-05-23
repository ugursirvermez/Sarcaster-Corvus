#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
using System;

namespace Unity.Services.Mediation.Platform
{
    class UnsupportedImpressionEventPublisher : IImpressionEventPublisher
    {
        public event EventHandler<ImpressionEventArgs> OnImpression;
    }
}
#endif
