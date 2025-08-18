using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class PetoonsMaterialsProjectSettings : ScriptableObject
    {
        public const string BUILD_STEPS_SETTINGS_FOLDERPATH = "Assets/Editor/ScriptableObjects/PetoonsProjectSettings";
        public const string BUILD_STEPS_SETTINGS_PATH = "Assets/Editor/ScriptableObjects/PetoonsProjectSettings/PetoonsMaterialsSettings.asset";

        private const string SHADERGRAPH_EXTENSION = ".shadergraph";
        private const string FBX_EXTENSION = ".fbx";

        [Information("Use to define the shader that will be applied to new materials.",InformationAttribute.InformationType.Info,false)]
        public bool Enabled = false;

        [HideInInspector]
        public List<string> ExcludeList = new List<string>();

        [HideInInspector]
        public List<string> ExcludeExtensionsList = new List<string>();

        [Tooltip("When a new material is created, if assigned, this shader will be used by the new material.")]
        [ConditionalHide("Enabled")]
        public Shader DefaultShaderMaterial;

        internal static PetoonsMaterialsProjectSettings GetOrCreateSettings()
        {
            if (!AssetDatabase.IsValidFolder(BUILD_STEPS_SETTINGS_FOLDERPATH))
            {
                PetoonsProcessorUtils.CreateDirectoryRecursive(BUILD_STEPS_SETTINGS_FOLDERPATH);
            }
            var settings = AssetDatabase.LoadAssetAtPath<PetoonsMaterialsProjectSettings>(BUILD_STEPS_SETTINGS_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<PetoonsMaterialsProjectSettings>();
                settings.ExcludeExtensionsList.Add(FBX_EXTENSION);
                settings.ExcludeExtensionsList.Add(SHADERGRAPH_EXTENSION);
                AssetDatabase.CreateAsset(settings, BUILD_STEPS_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public bool IsAssetExcluded(string assetPath)
        {
            foreach (var excludedAssetPath in ExcludeList)
            {
                if (assetPath.Contains(excludedAssetPath))
                    return true;
            }

            foreach (var extension in ExcludeExtensionsList)
            {
                if (assetPath.Contains(extension))
                    return true;
            }

            return false;
        }
    }

    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class PetoonsMaterialsProjectSettingsIMGUIRegister
    {
        private const string SETTINGS_PATH = "Project/Petoons/" + SETTINGS_LABEL;
        private const string SETTINGS_LABEL = "Materials";

        private static bool m_ExcludeFoldout = true;
        private static bool m_ExcludeExtensionFoldout = true;
        private static bool m_FunctionFoldout = false;

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
                    var settingsObject = PetoonsMaterialsProjectSettings.GetOrCreateSettings();
                    var editor = Editor.CreateEditor(settingsObject);
                    int indexToDelete = -1;
                    int indexExtensionToDelete = -1;

                    editor.DrawDefaultInspector();
                    
                    var serializedSettings = PetoonsMaterialsProjectSettings.GetSerializedSettings();
                    var serializedExcludeListProperty = serializedSettings.FindProperty("ExcludeList");
                    var serializedExcludeExtensionListProperty = serializedSettings.FindProperty("ExcludeExtensionsList");
                    serializedSettings.Update();

                    if (settingsObject.Enabled)
                    {
                        m_ExcludeFoldout = EditorGUILayout.Foldout(m_ExcludeFoldout, new GUIContent($"Exclude Paths List ({serializedExcludeListProperty.arraySize})", EditorGUIUtility.IconContent("Folder Icon").image, "List of string to exclude if contains."));
                        if (m_ExcludeFoldout)
                        {
                            DrawList(serializedExcludeListProperty, "Path", 40, ref indexToDelete);
                        }

                        m_ExcludeExtensionFoldout = EditorGUILayout.Foldout(m_ExcludeExtensionFoldout, new GUIContent($"Exclude Extensions List ({serializedExcludeExtensionListProperty.arraySize})", "List of extensions to exclude if contains.")); ;
                        if (m_ExcludeExtensionFoldout)
                        {
                            DrawList(serializedExcludeExtensionListProperty, "Extension", 60, ref indexExtensionToDelete);
                        }

                        EditorGUILayout.Space(5f);

                        m_FunctionFoldout = EditorGUILayout.Foldout(m_FunctionFoldout, new GUIContent("Other functions", EditorGUIUtility.IconContent("d_Material Icon").image));
                        if (m_FunctionFoldout)
                        {
                            EditorGUILayout.HelpBox("Use those functions with caution!", MessageType.Warning);
                            if (GUILayout.Button("Apply to current Project Materials"))
                            {
                                if (EditorUtility.DisplayDialog("Are you sure to apply Material Importer to current project Materials?",
                                    $"This change will process all project materials and apply the Materials Importer to avoid further import changes.",
                                    "No", "Yes"))
                                {
                                }
                                else
                                {
                                    MaterialsEditorProcessor.ApplyImportUserDataToExistingMaterials();
                                }
                            }
                            if (GUILayout.Button("Revert material changes"))
                            {
                                if (EditorUtility.DisplayDialog("Are you sure to revert Material Importer changes?",
                                    $"This change will process all project materials and revert the Materials Importer changes.",
                                    "No", "Yes"))
                                {
                                }
                                else
                                {
                                    MaterialsEditorProcessor.RevertMaterialImporterChanges();
                                }
                            }
                        }

                        serializedSettings.ApplyModifiedProperties();
                    }                   
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Material","Materials", "Shader", "Shaders" })
            };

            return provider;
        }

        private static void DrawList(SerializedProperty arrayProperty, string headerName, int headerSize, ref int indexToDelete)
        {
            if (arrayProperty.arraySize > 0)
            {
                EditorGUILayout.BeginVertical("Helpbox");
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(headerName, GUILayout.MaxWidth(headerSize));
                    EditorGUILayout.PropertyField(arrayProperty.GetArrayElementAtIndex(i), new GUIContent());
                    if (GUILayout.Button(EditorGUIUtility.IconContent("winbtn_win_close"), GUILayout.MaxWidth(20f)))
                    {
                        indexToDelete = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Toolbar Plus").image), EditorStyles.toolbarButton, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20f)))
            {
                arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            }
            GUILayout.Space(5f);

            EditorGUILayout.EndHorizontal();

            if (indexToDelete != -1)
            {
                arrayProperty.DeleteArrayElementAtIndex(indexToDelete);
                indexToDelete = -1;
            }
        }
    }


}
