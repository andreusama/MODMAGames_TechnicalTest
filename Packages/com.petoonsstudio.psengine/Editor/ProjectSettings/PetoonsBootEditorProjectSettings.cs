using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class PetoonsBootEditorProjectSettings : ScriptableObject
    {
        public const string EDITOR_BOOT_SETTINGS_FOLDERPATH = "Assets/Editor/ScriptableObjects/PetoonsProjectSettings";
        public const string EDITOR_BOOT_SETTINGS_PATH = "Assets/Editor/ScriptableObjects/PetoonsProjectSettings/PetoonsEditorBootSettings.asset";

        [Information("Determine wheter the Editor boot is active or not. Almost always should be active.", InformationAttribute.InformationType.Info, false)]
        public bool Enabled = true;

        [Information("Prefab with managers to start game from other scenes than Boot. This prefab must always be at Special Folder: Editor Default Resources", InformationAttribute.InformationType.Info, false)]
        public string EditorBootPrefabPath = "EditorBoot.prefab";

        [Information("Scene where this initialisation will not happen.", InformationAttribute.InformationType.Info, false)]
        public string BootScene = "Boot";


        internal static PetoonsBootEditorProjectSettings GetOrCreateSettings()
        {
            if (!AssetDatabase.IsValidFolder(EDITOR_BOOT_SETTINGS_FOLDERPATH))
            {
                PetoonsProcessorUtils.CreateDirectoryRecursive(EDITOR_BOOT_SETTINGS_FOLDERPATH);
            }
            var settings = AssetDatabase.LoadAssetAtPath<PetoonsBootEditorProjectSettings>(EDITOR_BOOT_SETTINGS_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<PetoonsBootEditorProjectSettings>();
                AssetDatabase.CreateAsset(settings, EDITOR_BOOT_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    static class PetoonsBootEditorProjectSettingsIMGUIRegister
    {
        private const string SETTINGS_PATH = "Project/Petoons/" + SETTINGS_LABEL;
        private const string SETTINGS_LABEL = "Editor Boot";

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = SETTINGS_LABEL,
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settingsObject = PetoonsBootEditorProjectSettings.GetOrCreateSettings();
                    var editor = Editor.CreateEditor(settingsObject);
                   

                    editor.DrawDefaultInspector();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Prefab", "Editor", "Boot" })
            };

            return provider;
        }
    }
}
