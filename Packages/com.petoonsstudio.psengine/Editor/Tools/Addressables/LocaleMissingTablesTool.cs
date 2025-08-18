using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.Tools
{
    public class LocaleMissingTablesTool : EditorWindow
    {
        // Single
        private LocalizationTableCollection m_Collection;
        private Vector2 m_Scroll;

        // Multiple
        private bool m_MultipleCollections;
        private string m_MultiCollPath;
        private List<LocalizationTableCollection> m_Collections;

        [MenuItem("Window/Petoons Studio/PSEngine/Localization/Locale Missing Tables", priority = ToolsUtils.LOCALIZATION_CATEGORY)]
        public static void Open()
        {
            // Get existing open window or if none, make a new one:
            LocaleMissingTablesTool window = (LocaleMissingTablesTool)EditorWindow.GetWindow(typeof(LocaleMissingTablesTool));
            window.titleContent = new GUIContent(nameof(LocaleMissingTablesTool));
            window.minSize = new Vector2(485f, window.minSize.y);
            window.Show();
            window.Setup();
        }

        private void Setup()
        {
            m_Collections = new List<LocalizationTableCollection>();
            FetchCollection();
        }

        private void OnGUI()
        {
            DrawModeToggle();

            GUILayout.Space(5);

            if (m_MultipleCollections)
            {
                MultipleCollectionsGUI();
            }
            else
            {
                SingleCollectionGUI();
            }
        }

        private void DrawModeToggle()
        {
            GUILayout.BeginHorizontal();
            m_MultipleCollections = !GUILayout.Toggle(!m_MultipleCollections, "Single Collection", EditorStyles.miniButtonLeft);
            m_MultipleCollections = GUILayout.Toggle(m_MultipleCollections, "Multiple Collections", EditorStyles.miniButtonRight);
            GUILayout.EndHorizontal();
        }

        private void SingleCollectionGUI()
        {
            GUILayout.Label($"Current Collection: {(m_Collection ? m_Collection.name : "None")}");

            if (GUILayout.Button("Fetch Collection") || m_Collection == null)
            {
                FetchCollection();
                return;
            }

            var missingTables = FindMissingLocales(m_Collection);

            if (missingTables.Count < 1)
            {
                GUILayout.Label("No missing tables :D");
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Missing Locale Tables:", EditorStyles.boldLabel);
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            GUILayout.BeginVertical();
            foreach (var missingTable in missingTables)
            {
                GUILayout.Label(missingTable.LocaleName);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(5);
            if (GUILayout.Button("Create missing tables"))
            {
                CreateMissingTables(m_Collection, missingTables, true);
            }
        }

        private void MultipleCollectionsGUI()
        {
            m_MultiCollPath = GUILayout.TextField(m_MultiCollPath);
            if (GUILayout.Button("Fetch all collections in path"))
            {
                FetchMultipleCollections();
                return;
            }

            if (m_Collections == null || m_Collections.Count < 1) return;

            GUILayout.Space(10);
            GUILayout.Label("Collections found:", EditorStyles.boldLabel);
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            GUILayout.BeginVertical();
            foreach (var collection in m_Collections)
            {
                GUILayout.Label(collection.name);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(5);
            if (GUILayout.Button("Create missing tables"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Do you want to create all missing tables for" +
                    $" the {m_Collections.Count} found table collections?", "Yes", "Nope"))
                {
                    for (int i = 0; i < m_Collections.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Creating Missing Tables for Collection", 
                            $"Creating tables for table {i} out of {m_Collections.Count} ({m_Collections[i].name})",
                            i / m_Collections.Count);

                        var missingTables = FindMissingLocales(m_Collections[i]);
                        if (missingTables.Count > 0) CreateMissingTables(m_Collections[i], missingTables, false);
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        private void FetchCollection()
        {
            if (Selection.activeObject is LocalizationTableCollection selectedCollection)
            {
                m_Collection = selectedCollection;
            }
        }

        private void FetchMultipleCollections()
        {
            m_Collections.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(LocalizationTableCollection)}", new[] { m_MultiCollPath });

            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            foreach (var path in paths)
            {
                m_Collections.Add((LocalizationTableCollection)AssetDatabase.LoadAssetAtPath(path, typeof(LocalizationTableCollection)));
            }
        }

        private void CreateMissingTables(LocalizationTableCollection collection, List<Locale> missingTables, bool showProgressBar)
        {
            for (int i = 0; i < missingTables.Count; i++)
            {
                if (showProgressBar) EditorUtility.DisplayProgressBar("Creating Missing Tables", $"Created {i} tables out of {missingTables.Count}", i / missingTables.Count);
                collection.AddNewTable(missingTables[i].Identifier);
            }
            if (showProgressBar) EditorUtility.ClearProgressBar();
        }

        private List<Locale> FindMissingLocales(LocalizationTableCollection collection)
        {
            List<Locale> missingLocales = new List<Locale>();

            // Find missing tables by project locales
            var projectLocales = LocalizationEditorSettings.GetLocales();
            
            foreach (var locale in projectLocales)
            {
                if (!collection.ContainsTable(locale.Identifier))
                    missingLocales.Add(locale);
            }

            return missingLocales;
        }
    } 
}
