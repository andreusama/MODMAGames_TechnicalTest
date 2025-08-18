using System.IO;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    /// <summary>
    /// This class adds a MoreMountains entry in Unity's top menu, allowing to enable/disable the help texts from the engine's inspectors
    /// </summary>
    public static class HelpMenu
    {

        [MenuItem(ToolsUtils.HELP_TOOLS_MENU + "Remove Saved Data")]
        /// <summary>
        /// Removes directory save files if default names are set
        /// </summary>
        private static void RemoveSavedData()
        {
            string savePath = Application.dataPath + "/SaveData";
            FileUtil.DeleteFileOrDirectory(savePath);
            AssetDatabase.Refresh();
        }

        [MenuItem(ToolsUtils.HELP_TOOLS_MENU + "Enable Help in Inspectors", false, 0)]
        /// <summary>
        /// Adds a menu item to enable help
        /// </summary>
        private static void EnableHelpInInspectors()
        {
            SetHelpEnabled(true);
        }

        [MenuItem(ToolsUtils.HELP_TOOLS_MENU + "Enable Help in Inspectors", true)]
        /// <summary>
        /// Conditional method to determine if the "enable help" entry should be greyed or not
        /// </summary>
        private static bool EnableHelpInInspectorsValidation()
        {
            return !HelpEnabled();
        }

        [MenuItem(ToolsUtils.HELP_TOOLS_MENU + "Disable Help in Inspectors", false, 1)]
        /// <summary>
        /// Adds a menu item to disable help
        /// </summary>
        private static void DisableHelpInInspectors()
        {
            SetHelpEnabled(false);
        }

        [MenuItem(ToolsUtils.HELP_TOOLS_MENU + "Disable Help in Inspectors", true)]
        /// <summary>
        /// Conditional method to determine if the "disable help" entry should be greyed or not
        /// </summary>
        private static bool DisableHelpInInspectorsValidation()
        {
            return HelpEnabled();
        }

        /// <summary>
        /// Checks editor prefs to see if help is enabled or not
        /// </summary>
        /// <returns><c>true</c>, if enabled was helped, <c>false</c> otherwise.</returns>
        private static bool HelpEnabled()
        {
            if (EditorPrefs.HasKey("MMShowHelpInInspectors"))
            {
                return EditorPrefs.GetBool("MMShowHelpInInspectors");
            }
            else
            {
                EditorPrefs.SetBool("MMShowHelpInInspectors", true);
                return true;
            }
        }

        /// <summary>
        /// Sets the help enabled editor pref.
        /// </summary>
        /// <param name="status">If set to <c>true</c> status.</param>
        private static void SetHelpEnabled(bool status)
        {
            EditorPrefs.SetBool("MMShowHelpInInspectors", status);
            SceneView.RepaintAll();

        }

        [MenuItem(ToolsUtils.HELP_TOOLS_MENU + "Create Asset Bundles")]
        /// <summary>
        /// Exports bundles
        /// </summary>
        public static void ExportBundle()
        {
#if UNITY_STANDALONE_WIN
            CreateBundle(Application.streamingAssetsPath + "/AssetBundles/Windows/", BuildTarget.StandaloneWindows64);
#elif UNITY_STANDALONE_OSX
            CreateBundle(Application.streamingAssetsPath + "/AssetBundles/OSX/", BuildTarget.StandaloneOSX);
#elif UNITY_PS4
            CreateBundle(Application.streamingAssetsPath + "/AssetBundles/PS4/", BuildTarget.PS4);
#elif UNITY_XBOXONE
            CreateBundle(Application.streamingAssetsPath + "/AssetBundles/Xbox/", BuildTarget.XboxOne);
#elif UNITY_SWITCH
            CreateBundle(Application.streamingAssetsPath + "/AssetBundles/Switch/", BuildTarget.Switch);
#endif
        }

        /// <summary>
        /// Create an asset bundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="target"></param>
        private static void CreateBundle(string path, BuildTarget target)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, target);
        }
    }
}