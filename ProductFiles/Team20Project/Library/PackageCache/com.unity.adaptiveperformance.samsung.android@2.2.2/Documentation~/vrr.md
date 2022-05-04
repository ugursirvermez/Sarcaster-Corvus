# Variable Refresh Rate API

The Variable Refresh Rate API allows you to change the current display refresh rate. The API provides `IVariableRefreshRate.SupportedRefreshRates`, an array of display refresh rates that the device supports. To change the current refresh rate, call `IVariableRefreshRate.SetRefreshRateByIndex` with a valid index for the array of supported refresh rates.

**Note:** The supported refresh rates depend on the model of the phone, Android Display Settings, and application-specific settings in Samsung Game Launcher.

The Variable Refresh Rate API is supported on all devices where `UnityEngine.AdaptivePerformance.Samsung.Android.VariableRefreshRate.Instance` is not `null`.

If the current refresh rate or the list of supported refresh rate changes because of an external event, the `IVariableRefreshRate.RefreshRateChanged` event is triggered. This can happen when a user is making changes to the Display Settings.

The Unity core API `Screent.currentResolution.refreshRate` is automatically updated when the refresh rate changes. This update might not happen immediately, so it is not recommended to cache the value of `Screent.currentResolution.refreshRate` in your application.

## Technical details

### Unity Support

This version of VRR is compatible with Unity Editor versions 2019 LTS and later, specifically:

* Unity 2020.2.0a7+
* Unity 2020.1.0b5+
* Unity 2019.3.11f1+

### Device Support

Variable Refresh Rate is currently only supported on following devices:

- Galaxy S20 with GameSDK 3.2 (April 2020 update).

## Project Settings

To enable proper timing for Adaptive Performance, you need to enable the **Frame Timing Stats** option (menu: **Edit &gt; Project Settings &gt; Player &gt; Other Settings**).

If you want to use `Application.targetFrameRate` to limit the target frame rate, set the **VSync Count** option under **Edit &gt; Project Settings &gt; Quality &gt; Other** to **Don't Sync**.

Unity has several quality levels in the quality settings. We recommend to switch the **VSync Count** to **Don't Sync** for each quality level. This will avoid issues with Adaptive Performance features like Adaptive Framerate and limiting the target framerate with `Application.targetFrameRate`.

### Optimized Frame Pacing

Adapitve Performance and Variable Refresh Rate is not compatible with framce pacing and we recommend to dissable **Optimized Frame Pacing** under **Edit &gt; Project Settings &gt; Player &gt; Resolution and Presentation**.
