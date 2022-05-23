namespace Unity.Services.Mediation
{
    /// <summary>
    /// The interface for Mediation features.
    /// </summary>
    public interface IMediationService
    {
        /// <summary>
        /// The Interstitial ads creator function.
        /// </summary>
        /// <param name="adUnitId"> The Ad Unit Id for the ad unit you wish to show. </param>
        /// <returns> A new Interstitial Ad instance. </returns>
        IInterstitialAd CreateInterstitialAd(string adUnitId);

        /// <summary>
        /// The Rewarded ads creator function.
        /// </summary>
        /// <param name="adUnitId"> The Ad Unit Id for the ad unit you wish to show. </param>
        /// <returns> A new Rewarded Ad instance. </returns>
        IRewardedAd CreateRewardedAd(string adUnitId);

        /// <summary>
        /// Access the Data Privacy API, to register the user's consent status.
        /// </summary>
        IDataPrivacy DataPrivacy { get; }

        /// <summary>
        /// Access the Impression Event Publisher API, to receive events when impression events are fired from ad objects.
        /// </summary>
        IImpressionEventPublisher ImpressionEventPublisher { get; }

        /// <summary>
        /// Native Mediation SDK version this mediation service is operating upon.
        /// </summary>
        string SdkVersion { get; }
    }
}
