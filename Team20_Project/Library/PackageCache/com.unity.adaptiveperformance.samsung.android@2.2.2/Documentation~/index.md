# About the Adaptive Performance Samsung (Android) package

The Adaptive Performance Samsung (Android) provider is a subsystem for [Adaptive Performance](https://docs.unity3d.com/Packages/com.unity.adaptiveperformance@latest/index.html) to extend Adaptive Performance to Samsung Android devices. It transmits device-specific information to the Adaptive Performance system and enables you to receive data about the thermal state of a Samsung Android device.

This package also provides access to a [Variable Refresh Rate](vrr.md) API that is supported by newer Samsung devices with high refresh rate displays.

## Installation

To use the Adaptive Performance Samsung (Android) provider you need to install the Adaptive Performance package and activate the provider in the Adaptive Performance section of the **Project Settings** window. For more information, see [Adaptive Performance package documentation](https://docs.unity3d.com/Packages/com.unity.adaptiveperformance@latest/index.html).

For information on what's new in the latest version of Adaptive Performance Samsung (Android), see the [Changelog](../changelog/CHANGELOG.html).

## Device support

This version of the Adaptive Performance Samsung (Android package) is compatible with Unity Editor versions 2019 LTS and later (2020.2 and later recommended).


Adaptive Performance Samsung (Android) currently supports the following Samsung devices running Android 10:

* All old and new Samsung Galaxy models

It supports those devices with Samsung GameSDK 3.2+.

|**Series**|**Models**|
|:---|:---|
|Galaxy S| Galaxy S10e / S10 / S10+<br> Galaxy S10 Lite<br> Galaxy S20 / S20+ / S20 Ultra<br> Galaxy S20 Lite<br> Galaxy S21 / S21+ / S21 Ultra |
|Galaxy Note| Galaxy Note9<br> Galaxy Note10 / Note10+<br> Galaxy Note10 Lite<br> Galaxy Note20 / Note20 Ultra|
|Galaxy Z| Galaxy Z Fold2<br> Galaxy Z Fold2|
|Galaxy A| Galaxy A10s<br> Galaxy A11<br> Galaxy A21s<br> Galaxy A31<br> Galaxy A41<br> Galaxy A50s<br> Galaxy A51<br> Galaxy A51 5G<br> Galaxy A71<br> Galaxy A71 5G<br> Galaxy A80<br> Galaxy A8 2018<br> Galaxy A9 2018<br> Galaxy A9 2018<br> Galaxy A90 5G|
|Galaxy Xcover| Galaxy Xcover 4s<br> Galaxy Xcover Pro|
|Galaxy M| Galaxy M11<br> Galaxy M30s<br> Galaxy M31s|
|Galaxy Tab| Galaxy Tab S6<br> Galaxy Tab S7<br>Galaxy Tab S7+|

Support status for each model could be different by the selling area and the OS version of the device.

[Variable Refresh Rate](vrr.md) is currently only supported on Galaxy S20, N20, S21.
Boost Mode and Cluster Info is currently only supported on Samsung GameSDK 3.5+ (Android 12)

For more information please also visit the [Samsung GameDev website](https://developer.samsung.com/galaxy-gamedev/adaptive-performance.html).

## Samsung GameSDK

When you enable logging, Adaptive Performance prints the version of the Samsung GameSDK used in the Adaptive Performance Samsung Android subsystem to the console during startup. For example:

```
Adaptive Performance: Subsystem version=3.2
```
