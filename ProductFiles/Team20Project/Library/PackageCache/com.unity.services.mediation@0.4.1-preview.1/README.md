
# About Unity Mediation
The Unity Mediation package (com.unity.services.mediation) offers a centralized way to implement mediation so you can monetize your app with both the Unity Ads network and multiple third-party ad networks.

This package provides:

* Support in the Unity Editor to install, update, and manage your ad network adapters
* APIs to load and display interstitial and rewarded ads in your app
* Monetization strategies like mediation waterfalls to help maximize your ad revenue
* Support to work with iOS and Android apps
* A sample project with mediation set up as an onboarding example

# Requirements
Unity Mediation is compatible with Unity Editor versions 2019.4 and later.

# Installing Unity Mediation
Open your Unity Editor and navigate to the Package Manager. Find the Mediation preview package and install the Mediation SDK. Alternatively, add the package via a tgz file or from your disk if you downloaded it from the Unity Dashboard.

# Using Unity Mediation
After installing the Mediation SDK, follow these steps to start using Unity Mediation in the Beta product phase:

1. Create an organization in the [Unity Dashboard](https://dashboard.unity3d.com/monetization).
2. Contact your Unity representative to get access to Mediation in the dashboard.
3. Select **Unity Mediation** as the Ads Provider for each project you want to set up to use Mediation.
4. Set up your ad sources by configuring third-party ad networks in the Unity Dashboard.
5. In your Unity project, go to **Project Settings** > **Services** > **Mediation** to open the Mediation Configuration page and install the ad network adapters for each ad source you configured in the Unity Dashboard. Then, open the Code Generator to select a platform and an ad unit to generate a sample code snippet. Copy the snippet and add it to your scene in the game object’s `monobehaviour` to load and show an ad.  

For more information, refer to the following docs: 
* [Unity Mediation Dashboard guide](http://documentation.cloud.unity3d.com/en/articles/5046463-unity-mediation-dashboard-guide)
* [Ad Source Configuration guide](http://documentation.cloud.unity3d.com/en/articles/5046485-unity-mediation-ad-source-configuration)
* [Made with Unity Integration guide](http://documentation.cloud.unity3d.com/en/articles/5046495-made-with-unity-mediation-integration-guide)
