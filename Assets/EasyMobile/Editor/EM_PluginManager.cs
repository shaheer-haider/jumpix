using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace EasyMobile.Editor
{
    [InitializeOnLoad]
    public class EM_PluginManager : AssetPostprocessor
    {
        #region Init

        // This static constructor will automatically run thanks to the InitializeOnLoad attribute.
        static EM_PluginManager()
        {
            EditorApplication.update += Initialize;
        }

        private static void Initialize()
        {
            EditorApplication.update -= Initialize;

            // Check if a new version has been imported and perform necessary updating jobs.
            VersionCheck();
        }

        #endregion

        #region Methods

        // Check if a *different* (maybe an older one is being imported!) version has been imported.
        // If yes, import the native package and update the version keys stored in settings file.
        internal static void VersionCheck()
        {
            int savedVersion = EM_ProjectSettings.Instance.GetInt(EM_Constants.PSK_EMVersionInt, -1);

            if (savedVersion != EM_Constants.versionInt)
            {
                // New version detected!
                EM_ProjectSettings.Instance.Set(EM_Constants.PSK_EMVersionString, EM_Constants.versionString);
                EM_ProjectSettings.Instance.Set(EM_Constants.PSK_EMVersionInt, EM_Constants.versionInt);

                // Import the Google Play Services Resolver
                ImportPlayServicesResolver(false);
            }
            else if (!IsPlayServicesResolverImported())
            {
                ImportPlayServicesResolver(false);
            }
        }

        internal static bool IsPlayServicesResolverImported()
        {
            return EM_ProjectSettings.Instance.GetBool(EM_Constants.PSK_ImportedPlayServicesResolver, false);
        }

        internal static void ImportPlayServicesResolver(bool interactive)
        {
            AssetDatabase.ImportPackage(EM_Constants.PlayServicersResolverPackagePath, interactive);
            EM_ProjectSettings.Instance.Set(EM_Constants.PSK_ImportedPlayServicesResolver, true);
        }

        #endregion
    }
}
