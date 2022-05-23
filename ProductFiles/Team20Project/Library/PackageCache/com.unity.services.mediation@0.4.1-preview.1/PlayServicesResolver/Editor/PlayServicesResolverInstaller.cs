using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlayServicesResolver.Installer.Editor
{
    static class PlayServicesResolverInstaller
    {
#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_PackagePath = "Assets/PlayServicesResolver/Editor/play-services-resolver-1.2.135.1.unitypackage";
#else
        const string k_PackagePath = "Packages/com.unity.services.mediation/PlayServicesResolver/Editor/play-services-resolver-1.2.135.1.unitypackage";
#endif

        const string k_DoNotAskAgain = "Unity.Mediation.PlayServicesResolver.DoNotAskAgain";

        [InitializeOnLoadMethod]
        static void InstallPlayServicesResolverIfNeeded()
        {
            EditorApplication.quitting += EditorApplicationOnQuitting;

            if (IsPlayServicesResolverInstalled())
                return;

            // The user will have a choice to ignore this dialog for the entire session.
            if (AskUserToInstallPackage())
            {
                InstallPackage();
            }
        }

        static void EditorApplicationOnQuitting()
        {
            EditorPrefs.DeleteKey(k_DoNotAskAgain);
        }

        static bool AskUserToInstallPackage()
        {
            if (EditorPrefs.GetBool(k_DoNotAskAgain)) return false;

            var response = EditorUtility.DisplayDialogComplex("Play Services Resolver required",
                "Mediation requires Play Services Resolver to resolve native dependencies.\n" +
                " Would you like to import the package?",
                "Import", "Cancel", "Ignore - Do not ask again during this session");

            switch (response)
            {
                case 0:
                    return true;
                case 1:
                    return false;
                case 2:
                    EditorPrefs.SetBool(k_DoNotAskAgain, true);
                    return false;
                default:
                    return false;
            }
        }

        internal static bool IsPlayServicesResolverInstalled()
        {
            try
            {
                return Type.GetType("Google.VersionHandler, Google.VersionHandler") != null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        static void InstallPackage()
        {
            var absolutePath = Path.GetFullPath(k_PackagePath);
            AssetDatabase.ImportPackage(absolutePath, false);
        }
    }
}
