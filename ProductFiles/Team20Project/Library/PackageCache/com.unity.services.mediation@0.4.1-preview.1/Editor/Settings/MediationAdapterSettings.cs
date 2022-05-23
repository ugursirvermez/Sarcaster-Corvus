using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Mediation.Adapters.Editor;
using UnityEditor;
using UnityEditor.Advertisements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class MediationAdapterSettings : EditorWindow
    {
        const string k_DashboardUrl     = @"https://dashboard.unity3d.com/monetization";
        const string k_DocumentationUrl = @"https://docs.unity3d.com/2020.2/Documentation/Manual/";
        const string k_Install          = "Install";
        const string k_Uninstall        = "Uninstall";
        const string k_ApiRangeTooltip  = "This adapter supports API versions: \n" + "Android: {0} to {1} \n" + "iOS: {2} to {3}";

#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_SettingsStyle       = @"Assets/UnityMonetization/Editor/Settings/Layout/SettingsStyle.uss";

        const string k_SettingsTemplate    = @"Assets/UnityMonetization/Editor/Settings/Layout/SettingsTemplate.uxml";
        const string k_AdapterTemplate     = @"Assets/UnityMonetization/Editor/Settings/Layout/AdapterTemplate.uxml";
#else
        const string k_SettingsStyle       = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/SettingsStyle.uss";

        const string k_SettingsTemplate    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/SettingsTemplate.uxml";
        const string k_AdapterTemplate     = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdapterTemplate.uxml";
#endif


#if UNITY_2020_1_OR_NEWER
        const string k_ServiceBaseStyle    = @"StyleSheets/ServicesWindow/ServicesProjectSettingsCommon.uss";
        static readonly string k_SkinStyle = $@"StyleSheets/ServicesWindow/ServicesProjectSettings{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
#else
    #if GAMEGROWTH_UNITY_MONETIZATION
        const string k_ServiceBaseStyle    = @"Assets/UnityMonetization/Editor/Settings/Layout/2019/BaseStyle.uss";
        static readonly string k_SkinStyle = $@"Assets/UnityMonetization/Editor/Settings/Layout/2019/SkinStyle{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
    #else
        const string k_ServiceBaseStyle    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/BaseStyle.uss";
        static readonly string k_SkinStyle = $@"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/SkinStyle{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
    #endif
#endif

        static Dictionary<string, AdapterInfo> s_AdapterInfos;
        static Dictionary<string, Toggle> s_AdapterSelectToggle;
        static Dictionary<string, VisualElement> s_AdapterInstalledInfo;
        static Dictionary<string, VisualElement> s_AdapterUninstalledInfo;
        static Dictionary<string, Button> s_AdapterInstallButton;
        static List<IAdapterSettings> s_AdapterSettings;
        static bool s_Initialized;
        static bool s_SettingsChanged;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            if (s_Initialized) return;
            s_Initialized = true;

            var adapters       = MediationSdkInfo.GetAllAdapters();
            s_AdapterInfos                    = adapters.ToDictionary(info => info.Identifier);
            s_AdapterSettings                 = FindAdapterSettings(adapters);
            s_AdapterSelectToggle             = new Dictionary<string, Toggle>();
            s_AdapterInstalledInfo            = new Dictionary<string, VisualElement>();
            s_AdapterUninstalledInfo          = new Dictionary<string, VisualElement>();
            s_AdapterInstallButton            = new Dictionary<string, Button>();
            MediationSdkInfo.AdaptersChanged += Refresh;
        }

        /// <summary>
        /// Refreshes the list of available adapters, installed adapters and installed versions
        /// Called on MediationSDKInfo.AdaptersChanged
        /// </summary>
        static void Refresh()
        {
            var installedAdapters = MediationSdkInfo.GetInstalledAdapters();
            var changed = false;

            foreach (var adapterSetting in s_AdapterSettings)
            {
                var adapterInfo = installedAdapters
                    .FirstOrDefault(info => info.Identifier == adapterSetting.AdapterId);
                var isInstalled = adapterInfo != null;

                if (isInstalled)
                {
                    if (adapterInfo.InstalledVersion.Identifier != adapterSetting.InstalledVersion.value)
                    {
                        adapterSetting.InstalledVersion.value = adapterInfo.InstalledVersion.Identifier;
                        changed = true;
                    }
                }
                else
                {
                    if (adapterSetting.InstalledVersion.value != "")
                    {
                        changed = true;
                        adapterSetting.InstalledVersion.value = "";
                    }
                }
            }

            if (changed)
                MediationSettingsProvider.instance.Save();

            RefreshAdaptersData();
            RefreshAllInstallButton();
        }

        [MenuItem("Services/Mediation/Configure", priority = 100)]
        public static void ShowWindow()
        {
            SettingsService.OpenProjectSettings("Project/Services/Mediation");
        }

        public static List<IAdapterSettings> FindAdapterSettings(List<AdapterInfo> adapters)
        {
            var adaptersIds = adapters.Select(info => info.Identifier).ToList();

            // Find all adapters that have a defined settings class
            var definedTypes =  typeof(MediationAdapterSettings).Assembly.GetTypes()
                .Where(type =>
                    typeof(IAdapterSettings).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    !typeof(GenericAdapterSettings).IsAssignableFrom(type))
                .ToList();

            // Create instances of the defined settings classes
            var instances = definedTypes
                .Select(type => (IAdapterSettings)Activator.CreateInstance(type))
                .ToList();

            // Any adapter that does not have a defined settings class, will use the generic settings class
            var adapterIdsWithGenericSettings = adaptersIds
                .Where(id => instances.All(settings => settings.AdapterId != id))
                .ToList();

            instances.AddRange(adapterIdsWithGenericSettings.Select(id => new GenericAdapterSettings(id)));

            //Sort based on original order of the adapters
            instances.Sort((settings1, settings2) =>
            {
                var i1 = adaptersIds.IndexOf(settings1.AdapterId);
                var i2 = adaptersIds.IndexOf(settings2.AdapterId);
                return i1 - i2;
            });

            return instances;
        }

        public static VisualElement GenerateUIElementUI()
        {
            VisualElement rootElement = new VisualElement();

            VisualTreeAsset settingsTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_SettingsTemplate);
            VisualTreeAsset adapterTemplate  = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdapterTemplate);

            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_SettingsStyle);
            rootElement.styleSheets.Add(stylesheet);

            stylesheet = EditorGUIUtility.Load(k_ServiceBaseStyle) as StyleSheet;
            rootElement.styleSheets.Add(stylesheet);

            stylesheet = EditorGUIUtility.Load(k_SkinStyle) as StyleSheet;
            rootElement.styleSheets.Add(stylesheet);

            settingsTemplate.CloneTree(rootElement);

            rootElement.Q<Button>("GoToAdUnits").clickable.clicked += MediationAdUnitsWindow.ShowWindow;
            rootElement.Q<Button>("GoToCodeGenerator").clickable.clicked += MediationCodeGeneratorWindow.ShowWindow;

            //Clear references to graphic elements as they will be generated here.
            s_AdapterSelectToggle.Clear();
            s_AdapterInstalledInfo.Clear();
            s_AdapterUninstalledInfo.Clear();
            s_AdapterInstallButton.Clear();

            // Fill the adapters list
            var adapterListRoot = rootElement.Q<VisualElement>("AdapterList");

            int i = 0;
            foreach (var adapterSetting in s_AdapterSettings)
            {
                var adapterInfo = s_AdapterInfos[adapterSetting.AdapterId];

                var adapter = new VisualElement();

                adapterTemplate.CloneTree(adapter);

                var adapterContainer = adapter.Q<VisualElement>("Adapter");

                string backgroundUssPrefix = EditorGUIUtility.isProSkin ? "dark" : "light";
                string rowUssPrefix = i % 2 == 0 ? "even" : "odd";

                adapterContainer.AddToClassList($"{backgroundUssPrefix}-{rowUssPrefix}-background");

                adapter.Q<TextElement>("AdapterName").text = adapterInfo.DisplayName;

                adapter.Q<Button>("InstallButton").clickable.clickedWithEventInfo += evt => AdapterInstallClicked(s_AdapterInstallButton.FirstOrDefault(pair => pair.Value == evt.target).Key);

                // Keep a reference to the graphic elements we will need to update.
                s_AdapterInstalledInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("InstalledInfo"));
                s_AdapterUninstalledInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("UninstalledInfo"));
                s_AdapterInstallButton.Add(adapterSetting.AdapterId, adapter.Q<Button>("InstallButton"));

                adapterSetting.OnAdapterSettingsGui("", adapter.Q<VisualElement>("Adapter"));

                adapterListRoot.Add(adapter);
                ++i;
            }

            RefreshAdaptersData();
            RefreshAllInstallButton();

#if !ENABLE_EDITOR_GAME_SERVICES
            MediationEditorService.RefreshGameId();
#endif
            rootElement.Q<TextElement>("android-game-id").text = AdvertisementSettings.GetGameId(RuntimePlatform.Android);
            rootElement.Q<TextElement>("ios-game-id").text     = AdvertisementSettings.GetGameId(RuntimePlatform.IPhonePlayer);

            return rootElement;
        }

        /// <summary>
        /// Refresh the values displayed in the adapters section
        /// </summary>
        static void RefreshAdaptersData()
        {
            foreach (var adapterSetting in s_AdapterSettings)
            {
                // If the UI has not been created yet, skip this adapter
                if (!s_AdapterInstalledInfo.ContainsKey(adapterSetting.AdapterId))
                    continue;

                var adapterInfo = s_AdapterInfos[adapterSetting.AdapterId];
                var isInstalled = !string.IsNullOrEmpty(adapterSetting.InstalledVersion.value);
                var installedVersionInfo = Array.Find(adapterInfo.Versions, x => x.Identifier == adapterSetting.InstalledVersion.value);
                s_AdapterInstalledInfo[adapterSetting.AdapterId].visible   = isInstalled;
                s_AdapterUninstalledInfo[adapterSetting.AdapterId].visible = !isInstalled;
                s_AdapterInstallButton[adapterSetting.AdapterId].visible = !(adapterSetting is UnityAdsSettings);

                RefreshAllInstallButton();
            }
        }

        /// <summary>
        /// Sets the appropriate text on the Install button ie Install or Uninstall for each adapter.
        /// </summary>
        static void RefreshAllInstallButton()
        {
            foreach (var adapterSetting in s_AdapterSettings)
            {
                var isInstalled = !string.IsNullOrEmpty(adapterSetting.InstalledVersion.value);

                if (s_AdapterInstallButton.ContainsKey(adapterSetting.AdapterId))
                {
                    s_AdapterInstallButton[adapterSetting.AdapterId].text = isInstalled ? k_Uninstall : k_Install;
                }
            }
        }

        /// <summary>
        /// Sets the appropriate text on the Install button ie Install or Uninstall for a specific adapter.
        /// </summary>
        static void RefreshInstallButton(string adapterIdentifier)
        {
            bool isInstalled = IsAdapterInstalled(adapterIdentifier);

            s_AdapterInstallButton[adapterIdentifier].text = isInstalled ? k_Uninstall : k_Install;
        }

        /// <summary>
        /// Installs/updates to a selected version of the adapter
        /// </summary>
        static void AdapterInstallClicked(string adapterIdentifier)
        {
            if (adapterIdentifier != default)
            {
                bool isInstalled = IsAdapterInstalled(adapterIdentifier);

                if (!isInstalled)
                {
                    MediationSdkInfo.Install(adapterIdentifier);
                }
                else
                {
                    MediationSdkInfo.Uninstall(adapterIdentifier);
                }

                RefreshInstallButton(adapterIdentifier);
            }
        }

        static bool IsAdapterInstalled(string adapterIdentifier)
        {
            return !string.IsNullOrEmpty(s_AdapterSettings.
                FirstOrDefault(adapterSettings => adapterSettings.AdapterId == adapterIdentifier)?.
                InstalledVersion.value);
        }
    }
}
