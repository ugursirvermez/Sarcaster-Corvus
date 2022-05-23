using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PlayServicesResolver.Utils.Editor
{
    static class PlayServicesResolverUtils
    {
        static Type s_PlayServicesResolverType;

        public static bool IsPresent => PlayServicesResolverType != null;

        public static void ResolveIfNeeded()
        {
            if (!AutomaticResolutionEnabled)
            {
                Resolve();
            }
        }

        public static bool AutomaticResolutionEnabled
        {
            get
            {
                var psrType = PlayServicesResolverType;
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("AutomaticResolutionEnabled");
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
        }

        public static bool GradleTemplateEnabled
        {
            get
            {
                var psrType = PlayServicesResolverType;
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("GradleTemplateEnabled");
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
        }

        public static bool MainTemplateEnabled
        {
            get
            {
                var psrType = Type.GetType("GooglePlayServices.SettingsDialog, Google.JarResolver");
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("PatchMainTemplateGradle", BindingFlags.Static | BindingFlags.NonPublic);
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
            set
            {
                var psrType = Type.GetType("GooglePlayServices.SettingsDialog, Google.JarResolver");
                if (psrType == null) return;
                var autoResolutionProperty = psrType.GetProperty("PatchMainTemplateGradle", BindingFlags.Static | BindingFlags.NonPublic);
                if (autoResolutionProperty == null) return;
                autoResolutionProperty.SetValue(null, value);
            }
        }

        public static void Resolve()
        {
            var psrType = PlayServicesResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("Resolve");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {Type.Missing, Type.Missing, Type.Missing});
        }

        public static void ResolveSync(bool forceResolution)
        {
            var psrType = PlayServicesResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("ResolveSync");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {forceResolution});
        }

        public static void DeleteResolvedLibraries()
        {
            var psrType = PlayServicesResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("DeleteResolvedLibrariesSync");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {});
        }

        public static IList<KeyValuePair<string, string>> GetPackageSpecs()
        {
            var psrType = PlayServicesResolverType;
            if (psrType == null) return new List<KeyValuePair<string, string>>();
            var getPackageSpecsMethod = psrType.GetMethod("GetPackageSpecs");
            if (getPackageSpecsMethod == null) return new List<KeyValuePair<string, string>>();
            return (IList<KeyValuePair<string, string>>)getPackageSpecsMethod.Invoke(null, new object[] { null });
        }

        static Type PlayServicesResolverType
        {
            get
            {
                if (s_PlayServicesResolverType != null)
                {
                    return s_PlayServicesResolverType;
                }

                try
                {
                    s_PlayServicesResolverType = Type.GetType("GooglePlayServices.PlayServicesResolver, Google.JarResolver");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return s_PlayServicesResolverType;
            }
        }
    }
}
