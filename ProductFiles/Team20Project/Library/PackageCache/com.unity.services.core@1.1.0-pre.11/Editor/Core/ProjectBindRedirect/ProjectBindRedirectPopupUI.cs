using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ProjectBindRedirect
{
    class ProjectBindRedirectPopupUI
    {
        const string k_ProjectSettingsPath = "Project/Services";

        Action m_OnCloseButtonFired;

        public ProjectBindRedirectPopupUI(VisualElement parentElement, Action closeAction)
        {
            SetupUxmlAndUss(parentElement);
            SetupButtons(parentElement);
            AddProjectBindRedirectContentUI(parentElement);

            EditorGameServiceSettingsProvider.TranslateStringsInTree(parentElement);

            m_OnCloseButtonFired = closeAction;
        }

        static void SetupUxmlAndUss(VisualElement containerElement)
        {
            var visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ProjectBindRedirectUiConstants.UxmlPath.Popup);
            if (visualAsset != null)
            {
                visualAsset.CloneTree(containerElement);
            }

            VisualElementHelper.AddStyleSheetFromPath(containerElement, ProjectBindRedirectUiConstants.UssPath.Popup);

            if (EditorGUIUtility.isProSkin)
            {
                VisualElementHelper.AddStyleSheetFromPath(containerElement, ProjectBindRedirectUiConstants.UssPath.PopupDark);
            }
            else
            {
                VisualElementHelper.AddStyleSheetFromPath(containerElement, ProjectBindRedirectUiConstants.UssPath.PopupLight);
            }
        }

        static void AddProjectBindRedirectContentUI(VisualElement parentElement)
        {
            var contentContainer = parentElement.Q(className: ProjectBindRedirectUiConstants.UxmlClassNames.ContentContainer) ?? parentElement;
            var contentUi = new ProjectBindRedirectContentUI(contentContainer);
        }

        void SetupButtons(VisualElement containerElement)
        {
            var cancelButton = containerElement.Q<Button>(className: ProjectBindRedirectUiConstants.UxmlClassNames.CancelButton);
            if (cancelButton != null)
            {
                cancelButton.clickable.clicked += CloseButtonAction;
            }

            var confirmButton = containerElement.Q<Button>(className: ProjectBindRedirectUiConstants.UxmlClassNames.ConfirmationButton);
            if (confirmButton != null)
            {
                confirmButton.clickable.clicked += ConfirmButtonAction;
            }
        }

        void CloseButtonAction()
        {
            m_OnCloseButtonFired?.Invoke();
        }

        void ConfirmButtonAction()
        {
            SettingsService.OpenProjectSettings(k_ProjectSettingsPath);
            CloseButtonAction();
        }
    }
}
