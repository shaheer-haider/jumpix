using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using UnityEditor.Graphs;

namespace EasyMobile.Editor
{
    public static class EM_MenuManager
    {

        #region Menu items

        [MenuItem("Window/" + EM_Constants.ProductName + "/Import Play Services Resolver", false, 4)]
        public static void MenuReimportNativePackage()
        {
            EM_PluginManager.ImportPlayServicesResolver(true);
        }

        [MenuItem("Window/" + EM_Constants.ProductName + "/User Guide", false, 5)]
        public static void OpenDocumentation()
        {
            Application.OpenURL(EM_Constants.DocumentationURL);
        }

        [MenuItem("Window/" + EM_Constants.ProductName + "/About", false, 6)]
        public static void About()
        {
            EditorWindow.GetWindow<EM_About>(true);
        }

        #endregion
    }
}