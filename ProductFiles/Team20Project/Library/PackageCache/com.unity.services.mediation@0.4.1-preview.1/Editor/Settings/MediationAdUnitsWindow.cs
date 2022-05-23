using System;
using System.Collections.Generic;
using Unity.Services.Mediation.Dashboard.Editor;
using Unity.Services.Mediation.Settings.Editor.Layout;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    /// <summary>
    /// Window listing the available Ad Units defined in the dashboard and their infos for the user's convenience
    /// </summary>
    class MediationAdUnitsWindow : EditorWindow
    {
        public static List<AdUnitData> AdUnitData => FilteredAdUnitData ?? AdUnitDataSource;

        private static List<AdUnitData> AdUnitDataSource = new List<AdUnitData>();
        private static List<AdUnitData> FilteredAdUnitData;

        private static SortMode[] SortModes = {SortMode.Ascending, SortMode.Ascending, SortMode.Ascending, SortMode.Ascending};

#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_AdUnitsTemplate = @"Assets/UnityMonetization/Editor/Settings/Layout/AdUnitsTemplate.uxml";
        const string k_AdUnitsStyle    = @"Assets/UnityMonetization/Editor/Settings/Layout/AdUnitsStyle.uss";

        const string k_AdUnitsWarningTemplate = @"Assets/UnityMonetization/Editor/Settings/Layout/AdUnitsWarningTemplate.uxml";
        const string k_AdUnitsErrorTemplate   = @"Assets/UnityMonetization/Editor/Settings/Layout/AdUnitsErrorTemplate.uxml";
#else
        const string k_AdUnitsTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsTemplate.uxml";
        const string k_AdUnitsStyle    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsStyle.uss";

        const string k_AdUnitsWarningTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsWarningTemplate.uxml";
        const string k_AdUnitsErrorTemplate   = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsErrorTemplate.uxml";
#endif

        [MenuItem("Services/Mediation/Ad Units", priority = 111)]
        public static void ShowWindow()
        {
            GetWindow<MediationAdUnitsWindow>("Mediation - Ad Units", new Type[] { typeof(MediationCodeGeneratorWindow), typeof(SceneView), typeof(EditorWindow)});
        }

        void OnFocus()
        {
            if (rootVisualElement.Q(className: "list-view") != null)
            {
                RetrieveAdUnitInfo(rootVisualElement);
            }
            else
            {
                RefreshWindow();
            }
        }

        private void RefreshWindow()
        {
            rootVisualElement.Clear();

            AdUnitVisualElement.Initialize();

            VisualElement root = rootVisualElement;
            AddStyleSheets(root);

            VisualTreeAsset adUnitTemplateAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdUnitsTemplate);
            adUnitTemplateAsset.CloneTree(root);

            RetrieveAdUnitInfo(root);
        }

        private void RetrieveAdUnitInfo(VisualElement root)
        {
            DashboardClient.GetAdUnitsAsync(adUnits =>
            {
                // Remove Loading Element
                root.Q<Label>("loading")?.RemoveFromHierarchy();

                // Can't fetch ad unit data, display error message box
                if (adUnits == null)
                {
                    root.Q(className: "list-view")?.RemoveFromHierarchy();

                    VisualTreeAsset errorTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdUnitsErrorTemplate);
                    errorTemplate.CloneTree(root.Q(className: "table-box"));
                }
                else if (adUnits.Length == 0)
                {
                    root.Q(className: "list-view")?.RemoveFromHierarchy();

                    VisualTreeAsset warningTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdUnitsWarningTemplate);
                    warningTemplate.CloneTree(root.Q(className: "table-box"));
                }
                else
                {
                    // Received data construct ad units list.
                    List<AdUnitData> dashboardAdUnits = new List<AdUnitData>();
                    foreach (DashboardClient.AdUnitInfoJson adUnitInfo in adUnits)
                    {
                        if (!adUnitInfo.isArchived)
                        {
                            dashboardAdUnits.Add(new AdUnitData(adUnitInfo));
                        }
                    }
                    AdUnitDataSource = dashboardAdUnits;
                    ConstructListFromAdUnits(root);
                }
            });
        }

        static SortMode GetNextSortMode(SortMode sortMode)
        {
            return (SortMode)(((int)sortMode + 1) % 2);
        }

        private static EventCallback<ChangeEvent<string>> FilterListBySearchField(ListView listView)
        {
            return evt =>
            {
                if (!string.IsNullOrEmpty(evt?.newValue))
                {
                    FilteredAdUnitData =
                        AdUnitDataSource.FindAll(data => data.AdUnit.ToLower().Contains(evt.newValue.ToLower()));
                    listView.itemsSource = FilteredAdUnitData;
                }
                else
                {
                    FilteredAdUnitData = null;
                    listView.itemsSource = AdUnitDataSource;
                }
            };
        }

        private static void SortColumn(ListView listView, int columnIndex, Comparison<AdUnitData> comparisonFunction)
        {
            SortModes[columnIndex] = GetNextSortMode(SortModes[columnIndex]);
            switch (SortModes[0])
            {
                case SortMode.Descending:
                    FilteredAdUnitData = AdUnitData;
                    FilteredAdUnitData.Sort(comparisonFunction);
                    listView.itemsSource = FilteredAdUnitData;
                    break;
                case SortMode.Ascending:
                    FilteredAdUnitData = AdUnitData;
                    FilteredAdUnitData.Sort(comparisonFunction);
                    listView.itemsSource = FilteredAdUnitData;
                    break;
            }
        }

        private static void ConstructListFromAdUnits(VisualElement root)
        {
            ListView listView = root.Q<ListView>(className: "list-view");

            listView.makeItem = AdUnitVisualElement.CreateListItem;
            listView.bindItem = AdUnitVisualElement.BindListItem;
            listView.itemsSource = AdUnitDataSource;

            ToolbarSearchField searchField = root.Q<ToolbarSearchField>(className: "search-field");
            searchField.RegisterValueChangedCallback(FilterListBySearchField(listView));

            root.Q<Label>("list-header-adunit").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 0, (data, unitData) => Editor.AdUnitData.CompareByAdUnit(SortModes[0], data, unitData));
            });

            root.Q<Label>("list-header-platform").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 1, (data, unitData) => Editor.AdUnitData.CompareByPlatform(SortModes[1], data, unitData));
            });

            root.Q<Label>("list-header-adformat").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 2, (data, unitData) => Editor.AdUnitData.CompareByAdFormat(SortModes[2], data, unitData));
            });

            root.Q<Label>("list-header-id").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 3, (data, unitData) => Editor.AdUnitData.CompareById(SortModes[3], data, unitData));
            });
        }

        private void AddStyleSheets(VisualElement root)
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_AdUnitsStyle);
            root.styleSheets.Add(styleSheet);
#if GAMEGROWTH_UNITY_MONETIZATION
            string k_SkinStyle = $@"Assets/UnityMonetization/Editor/Settings/Layout/2019/SkinStyle{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
#else
            string k_SkinStyle = $@"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/SkinStyle{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
#endif
            styleSheet = EditorGUIUtility.Load(k_SkinStyle) as StyleSheet;
            root.styleSheets.Add(styleSheet);
        }
    }
}
