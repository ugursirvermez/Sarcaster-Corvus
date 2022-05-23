# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.4.1-preview.1] - 2022-03-29

### Fixed
- Android: Fixed issue with some devices not supporting a default constructor for S2SRedeemData through JNI.

### Changed
- iOS: Version restriction for the adapters in anticipation for a dependency tree structure change.


## [0.4.0-preview.2] - 2022-02-16

### Fixed
- Added Optional ShowOptions parameter to UnsupportedRewardedAd

## [0.4.0-preview.1] - 2022-02-01
### Added
- Server to server redeem callback support.
    - RewardedAdShowOptions class to pass optional arguments when showing a rewarded ad.
    - S2SRedeemData struct to pass server to server redeem data.

### Changed
- ImpressionData.PublisherRevenuePerImpression type changed from string to double.
- ImpressionData.PublisherRevenuePerImpressionInMicros type changed from string to Int64.
- RewardedAd's Show function now accepts an optional RewardedAdShowOptions parameter.

### Fixed
- Editor mock rewarded ads will no longer trigger the reward callback if they are skipped.

## [0.3.0-preview.3] - 2021-12-01

### Added
- MediationService.Instance.SdkVersion: Gets the native Mediation version at runtime. 
- Line numbers for generated code in the Code Generation Window.
- PIPL Support for DataPrivacy Laws Enum
    - DataPrivacy.PIPLAdPersonalization - Personal Information Protection Law, regarding ad personalization, applicable to users residing in China.
    - DataPrivacy.PIPLDataTransport - Personal Information Protection Law, regarding moving data out of China, applicable to users residing in China

### Changed
- Refined Code Generation Code
    - Class Name includes Ad Type to avoid class name conflicts. (MyExampleAdClass -> InterstitialAdExample)
    - Uses override game id initialization flow now
    - Added OnClicked and OnClosed Callbacks to code snippet.
    - Fixed Newline issues with OnUserRewarded Callback placement.
    - Renamed Event Args parameter in Ad Loaded (sargs -> args)
    - OnUserRewarded subscribed method renamed (OnUserRewarded -> UserRewarded)
    - Code snippet color changed to increase visibility.
- Banner ads will now be excluded from the code generator, as they are not supported at the moment.
    
### Fixed
- Unity Ads sometimes not appearing as installed in the Mediation Settings Window.
- Misalignment of adapter status and its icon on Windows for Unity 2020+
- Dark color palettes used in some Mediation UI when using the Light Theme. 

## [0.2.1-preview.2] - 2021-10-12

### Added
- Test validating gradle version to display a more meaningful error message.
- `InitializationOptions` extension `SetGameId` to manually specify a game id when initializing mediation.

### Changed
- Overhauled the Mediation Settings UI.
    - Uninstalled indicators
    - Alternating backgrounds
    - Game id display for game ids retrieved from the Dashboard
- In-Editor Test Ads: Color removed from console logs

### Fixed
- Archived Ad Units will no longer be displayed in the ad units list.
- In-Editor Test Ads would not initialize if the build target was not supported by mediation.
- Removed error when importing play services resolver for the first time.
