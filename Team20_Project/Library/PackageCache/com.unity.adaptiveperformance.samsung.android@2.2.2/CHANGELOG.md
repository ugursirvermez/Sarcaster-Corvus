# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.2.2] - 2022-01-10

### Added
- Additional logging for the initialization phase of the provider.
- Added additional documentation for supported devices.

## [2.2.1] - 2021-06-06

### Changed
- Scalers do not use switch-case to determine their value, instead use min, max scale and max level to calculate a scale and increment and apply it to their target when the level changes. VRR scaler specifically.

### Added
- Boost mode
- Cluster info

## [2.1.1] - 2021-02-03

### Changed
- Fix FB: 1305455 IndexOutOfRangeException error thrown on changing VRR Refresh Rate on device.
- Fix FB: 1309052 IndexOutOfRangeException error thrown on changing VRR Refresh Rate on Device Simulator.

## [2.1.0] - 2020-10-12

### Changed
- Updated the version defines for the device simulator to support it in 2021.1 without package.
- Updated the version to keep in sync with the main Adaptive Performance package.

## [2.0.2] - 2020-08-21

### Changed
- Improved Stats reporting.

### Removed
- Folders and files which are not needed by Adaptive Performance from the package.

## [2.0.1] - 2020-08-10

### Changed
- Updated GameSDK wrapper to latest version which enhances GPU frametime information

## [2.0.0] - 2020-06-05

### Added
- Variable Refresh Rate API (VRR) to support multiple vsync displays
- Verified support for 2020.2 and minimum support 2019 LTS+. Please use Adaptive Performance Samsung Android 1.x for earlier versions supporting 2018.3+.

## [1.2.2] - 2021-02-05

### Changed
- Fix issues where skin temperature was used on Game SDK 1.5 devices instead of PST to send initial temperature warning when devices are hot.

## [1.2.1] - 2020-08-21

### Changed
- Update to correct Unity Version (2019.4) and correct dependency to Adaptive Performance (1.2.0).
- Updated GameSDK wrapper to latest version which enhances GPU frametime information.

## [1.2.0] - 2020-07-29

### Changed
- Updates to Subsystem Registry 1.2.0 to fix installation issues if internal subystem module is disabled.
- Update minimum required Unity version to 2019.4.

## [1.1.9] - 2020-07-23

### Changed
- Exchanged GameSDK wrapper with updated version removing GameSDK 3.1 support.
- Thermal Mitigation Logic changes in GameSDK 3.2 and it was updated in SetFreqLevels() to react to the correct return values.
- Thermal Mitigation Logic sets Automatic Performance Control to System when not available and releases when it reaches norminal temperature levels.
- Automatic Performance Control does not lower CPU lower than 1 on GameSDK 3.2 workaround.
- Add workaround to send temperature warning when the device starts as warm already as currently no events are sent.

## [1.1.7] - 2020-05-07

### Changed
- Adaptive Performance needs to re-initialize GameSDK on resume application because some Android onForegroundchange() APIs do not detect the change (e.g. bixby) and cause Adaptive Performance to not get valid data anymore.

## [1.1.6] - 2020-04-29

### Changed
- GameSDK 3.2 uses a wider range of temperature levels and maximum temperature level is changed to level 10.
- GameSDK 3.2 has a different behaviour when setting frequency levels and warning level 2 (throttling) is reached and you are always in control of the CPU/GPU level.

## [1.1.4] - 2020-03-26

### Removed
- Game SDK 3.1 initialization due to issues in GameSDK 3.1. Any other GameSDK version is still supported.

## [1.1.3] - 2020-03-18

### Fixed
- Avoids that callbacks in GameSDK 3.1, such as Listener.onHighTempWarning(), are not called when onRefreshRateChanged() is not implemented. This is only present on devices supporting VRR

### Changed
-With GameSDK 3.1 it's not necessary to (un)register listeners during OnPause and OnResume as it's handled in the GameSDK

## [1.1.2] - 2020-03-13

### Added
- Updated GameSDK from 3.0 to 3.1

### Fixed
- Avoid onRefreshRateChanged() crash on S20 during Motion smoothness change (60Hz <-> 120Hz) while app is in background and resumed

### Improvement
- GameSDK 3.1 introduced setFrequLevel callback for temperature mitigation to avoid overheating when no additional scale factors are used. This replaces SetLevelWithScene in GameSDK 3.1

## [1.1.1] - 2020-02-13

### Changed
- Package name from AP Samsung Android to Adaptive Performance Samsung Android as the Unity Package Manager naming limit was raised to 50 characters

## [1.1.0] - 2020-01-30

### Fixed
- Compatibility with .net 3.5 in Unity 2018.4

## [1.0.1] - 2019-08-29

### Changed
- Compatibility with Subsystem API changes in Unity 2019.3

## [1.0.0] - 2019-08-19

### Added
- Support for Samsung GameSDK 3.0

## [0.2.0-preview.1] - 2019-06-19

### This is the first preview release of the *Adaptive Performance Samsung (Android)* package for *Adaptive Performance* which was integrated within Adaptive Performance previously.
