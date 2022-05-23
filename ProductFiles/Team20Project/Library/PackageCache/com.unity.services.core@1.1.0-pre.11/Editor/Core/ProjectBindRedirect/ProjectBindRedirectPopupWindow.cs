using UnityEditor;
using UnityEngine;

namespace Unity.Services.Core.Editor.ProjectBindRedirect
{
    class ProjectBindRedirectPopupWindow : EditorWindow
    {
        internal static readonly Vector2 PopupSize = new Vector2(600, 400);
        const string k_WindowTitle = "Link your Unity project";

        ProjectBindRedirectPopupUI m_PopupUI;

        void Update()
        {
            if (RequiresInitialization())
            {
                Initialize();
            }
        }

        bool RequiresInitialization()
        {
            return m_PopupUI == null;
        }

        internal static ProjectBindRedirectPopupWindow CreateAndShowPopup()
        {
            var popupWindow = GetWindow<ProjectBindRedirectPopupWindow>(k_WindowTitle);
            popupWindow.Initialize();

            return popupWindow;
        }

        void Initialize()
        {
            rootVisualElement?.Clear();

            m_PopupUI = new ProjectBindRedirectPopupUI(rootVisualElement, Close);

            maxSize = minSize = PopupSize;
        }
    }
}
