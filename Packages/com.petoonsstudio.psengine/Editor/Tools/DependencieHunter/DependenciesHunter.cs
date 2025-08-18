#define HUNT_ADDRESSABLES

using DG.Tweening.Plugins.Core.PathCore;
using PetoonsStudio.PSEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.Build;
using UnityEditor.PackageManager.UI;
#endif
#if HUNT_ADDRESSABLES
using UnityEditor.AddressableAssets;
#endif
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;
using Path = System.IO.Path;

//From: https://github.com/AlexeyPerov/Unity-Dependencies-Hunter
// ReSharper disable once CheckNamespace
namespace PetoonsStudio.PSEngine.Tools
{
    /// <summary>
    /// Lists all references of the selected assets.
    /// </summary>
    public class SelectedAssetsReferencesWindow : EditorWindow
    {
        private SelectedAssetsAnalysisUtilities _service;
        private FindUnusedAssets m_UnusedAssetsFinder;
        private int m_CurrentTool = 0;
        private string[] m_Tools = new string[] { "Dependencies", "No Dependencies" };

        private const float TabLength = 60f;

        private Dictionary<string, List<string>> _lastResults;

        private Object[] _selectedObjects;

        private bool[] _selectedObjectsFoldouts;

        private float _workTime;

        private Vector2 _scrollPos = Vector2.zero;
        private Vector2[] _foldoutsScrolls;
        private DependenciesHunterInformation DependenciesInformation;


        [MenuItem("Assets/Find References In Project", false, 20)]
        [MenuItem("Window/Petoons Studio/PSEngine/Editor/Dependencies Hunter", false, 20)]
        public static void FindReferences()
        {
            var window = GetWindow<SelectedAssetsReferencesWindow>();
            window.titleContent = new GUIContent("Dependencies Hunter");
            window.Start();
        }

        private void Start()
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (_service == null)
            {
                _service = new SelectedAssetsAnalysisUtilities();
            }
            DependenciesInformation = GetDependenciesInformationAsset();

            Show();

            if (_service == null)
            {
                _service = new SelectedAssetsAnalysisUtilities();
            }

            if (DependenciesInformation.MapListCache.Count > 0)
            {
                _service.CachedAssetsMap = DependenciesInformation.GetDictionary();
            }

            var startTime = Time.realtimeSinceStartup;

            _selectedObjects = Selection.objects;
            _lastResults = _service.GetReferences(_selectedObjects);
            DependenciesInformation.AddCacheMap(_service.CachedAssetsMap);

            EditorUtility.DisplayProgressBar("DependenciesHunter", "Preparing Assets", 1f);
            EditorUtility.UnloadUnusedAssetsImmediate();
            EditorUtility.ClearProgressBar();

            _workTime = Time.realtimeSinceStartup - startTime;
            _selectedObjectsFoldouts = new bool[_selectedObjects.Length];
            if (_selectedObjectsFoldouts.Length == 1)
            {
                _selectedObjectsFoldouts[0] = true;
            }

            _foldoutsScrolls = new Vector2[_selectedObjectsFoldouts.Length];

            m_UnusedAssetsFinder = new FindUnusedAssets(DependenciesInformation);
        }

        private DependenciesHunterInformation GetDependenciesInformationAsset()
        {
            var editorInformation = AssetDatabase.FindAssets("t:DependenciesHunterInformation");
            if (editorInformation.Length <= 0)
            {
                var asset = CreateInstance<DependenciesHunterInformation>();
                if (!AssetDatabase.IsValidFolder("Assets/Editor/PersistenceEditorAssets/"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Editor/PersistenceEditorAssets/");
                }

                AssetDatabase.CreateAsset(asset, "Assets/Editor/PersistenceEditorAssets/DependencieHunter.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return asset;
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<DependenciesHunterInformation>(AssetDatabase.GUIDToAssetPath(editorInformation[0]));
            }
        }

        private void Clear()
        {
            _selectedObjects = null;
            _service = null;

            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        private void OnGUI()
        {
            if (_lastResults == null)
            {
                return;
            }

            if (_selectedObjects == null || _selectedObjects.Any(selectedObject => selectedObject == null))
            {
                Clear();
                return;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            m_CurrentTool = GUILayout.Toolbar(m_CurrentTool, m_Tools);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Update"), EditorStyles.toolbarButton, GUILayout.MaxWidth(100)))
            {
                switch (m_CurrentTool)
                {
                    case 0:
                        Start();
                        break;
                    case 1:
                        m_UnusedAssetsFinder.UpdateUnusedAssets(DependenciesInformation);
                        break;
                    default:
                        break;
                }
            }

            if (GUILayout.Button(new GUIContent("Refresh", "Recreate asset reference cache, this will take some time"), EditorStyles.toolbarButton, GUILayout.MaxWidth(100)))
            {
                _service.CachedAssetsMap = null;
                DependenciesInformation.Clear();
                Start();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5f); 

            GUILayout.BeginVertical();

            switch (m_CurrentTool)
            {
                case 0:
                    DrawDependencieHunter();
                    break;
                case 1:
                    m_UnusedAssetsFinder.DrawGUI();
                    break;
                default:
                    break;
            }
            GUILayout.EndVertical();
        }

        private void DrawDependencieHunter()
        {
            GUILayout.Label($"Dependencies Hunter");
            GUILayout.Label($"-Analysis done in: {_workTime} s");
            EditorGUILayout.HelpBox("USE WITH CAUTION: This tool caches the map dependencies of every asset of the project. If you change any asset, please use the refresh button to recreate it. To relaunch the search with another asset, select another asset and click Update. This tool will create an asset in editor, you can upload it to Git in order to do quick searches.", MessageType.Warning);
            var results = _lastResults;

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            for (var i = 0; i < _selectedObjectsFoldouts.Length; i++)
            {
                GUIUtilities.HorizontalLine();

                GUILayout.BeginHorizontal();
                var selectedObjectPath = AssetDatabase.GetAssetPath(_selectedObjects[i]);
                string selectedGuidObject = AssetDatabase.AssetPathToGUID(selectedObjectPath, AssetPathToGUIDOptions.OnlyExistingAssets);

                var dependencies = results[selectedGuidObject];
                _selectedObjectsFoldouts[i] = EditorGUILayout.Foldout(_selectedObjectsFoldouts[i], string.Empty);
                EditorGUILayout.ObjectField(_selectedObjects[i], typeof(Object), true);

                var content = dependencies.Count > 0 ? $"Dependencies: {dependencies.Count}" : "No dependencies found";
                EditorGUILayout.LabelField(content);

                GUILayout.EndHorizontal();

                if (_selectedObjectsFoldouts[i])
                {
                    _foldoutsScrolls[i] = GUILayout.BeginScrollView(_foldoutsScrolls[i]);

                    foreach (var guid in dependencies)
                    {
                        var resultPath = AssetDatabase.GUIDToAssetPath(guid);
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Space(TabLength);

                        var type = AssetDatabase.GetMainAssetTypeAtPath(resultPath);
                        var guiContent = EditorGUIUtility.ObjectContent(null, type);
                        guiContent.text = Path.GetFileName(resultPath);

                        var alignment = GUI.skin.button.alignment;
                        GUI.skin.button.alignment = TextAnchor.MiddleLeft;

                        if (GUILayout.Button(guiContent, GUILayout.MinWidth(300f), GUILayout.Height(18f)))
                        {
                            Selection.objects = new[] { AssetDatabase.LoadMainAssetAtPath(resultPath) };
                        }

                        GUI.skin.button.alignment = alignment;

                        EditorGUILayout.EndHorizontal();
                    }

                    GUILayout.EndScrollView();
                }
            }

            GUILayout.EndScrollView();
        }

        private void OnProjectChange()
        {
            Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }

    public class ProjectAssetsAnalysisUtilities
    {
        private List<string> _iconPaths;

        public bool IsValidAssetType(string path, bool validForOutput)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);

            if (type == null)
            {
                if (validForOutput)
                    Debug.LogWarning($"Invalid asset type found at {path}");
                return false;
            }

            if (type == typeof(MonoScript) || type == typeof(DefaultAsset))
            {
                return false;
            }

            if (type == typeof(SceneAsset))
            {
                var scenes = EditorBuildSettings.scenes;

                if (scenes.Any(scene => scene.path == path))
                {
                    return false;
                }
            }

            return type != typeof(Texture2D) || !UsedAsProjectIcon(path);
        }

        public static bool IsValidForOutput(string path, List<string> ignoreInOutputPatterns)
        {
            return ignoreInOutputPatterns.All(pattern
                => string.IsNullOrEmpty(pattern) || !Regex.Match(path, pattern).Success);
        }

        private bool UsedAsProjectIcon(string texturePath)
        {
            if (_iconPaths == null)
            {
                FindAllIcons();
            }

            return _iconPaths.Contains(texturePath);
        }

        private void FindAllIcons()
        {
            _iconPaths = new List<string>();

            var icons = new List<Texture2D>();

#if UNITY_2021_2_OR_NEWER
            foreach (var buildTargetField in typeof(NamedBuildTarget).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (buildTargetField.Name == "Unknown")
                    continue;
                if (buildTargetField.FieldType != typeof(NamedBuildTarget))
                    continue;

                NamedBuildTarget buildTarget = (NamedBuildTarget)buildTargetField.GetValue(null);
                icons.AddRange(PlayerSettings.GetIcons(buildTarget, IconKind.Any));
            }
#else
            foreach (var targetGroup in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                icons.AddRange(PlayerSettings.GetIconsForTargetGroup((BuildTargetGroup) targetGroup));
            }
#endif

            foreach (var icon in icons)
            {
                _iconPaths.Add(AssetDatabase.GetAssetPath(icon));
            }
        }
    }

    public class SelectedAssetsAnalysisUtilities
    {
        private Dictionary<string, List<string>> _cachedAssetsMap;

        public Dictionary<string, List<string>> CachedAssetsMap { get => _cachedAssetsMap; set => _cachedAssetsMap = value; }

        public Dictionary<string, List<string>> GetReferences(Object[] selectedObjects)
        {
            if (selectedObjects == null)
            {
                Debug.Log("No selected objects passed");
                return new Dictionary<string, List<string>>();
            }

            if (_cachedAssetsMap == null)
            {
                DependenciesMapUtilities.FillReverseDependenciesMap(out _cachedAssetsMap);
            }

            EditorUtility.ClearProgressBar();

            GetDependencies(selectedObjects, _cachedAssetsMap, out var result);

            return result;
        }

        private static void GetDependencies(IEnumerable<Object> selectedObjects, IReadOnlyDictionary<string,
            List<string>> source, out Dictionary<string, List<string>> results)
        {
            results = new Dictionary<string, List<string>>();

            foreach (var selectedObject in selectedObjects)
            {
                var selectedObjectPath = AssetDatabase.GetAssetPath(selectedObject);
                string guid = AssetDatabase.AssetPathToGUID(selectedObjectPath, AssetPathToGUIDOptions.OnlyExistingAssets);

                if (source.ContainsKey(guid))
                {
                    results.Add(guid, source[guid]);
                }
                else
                {
                    Debug.LogWarning("Dependencies Hunter doesn't contain the specified object in the assets map",
                        selectedObject);
                    results.Add(guid, new List<string>());
                }
            }
        }
    }

    public static class DependenciesMapUtilities
    {
        public static void FillReverseDependenciesMap(out Dictionary<string, List<string>> reverseDependencies)
        {
            var assetPaths = AssetDatabase.GetAllAssetPaths().ToList();
            List<string> assetGuids = new List<string>();
            foreach (var item in assetPaths)
            {
                assetGuids.Add(AssetDatabase.AssetPathToGUID(item, AssetPathToGUIDOptions.OnlyExistingAssets));
            }


            reverseDependencies = assetGuids.ToDictionary(assetGuids => assetGuids, assetGuids => new List<string>());

            Debug.Log($"Total Assets Count: {assetGuids.Count}");

            for (var i = 0; i < assetGuids.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Dependencies Hunter", "Creating a map of dependencies",
                    (float)i / assetGuids.Count);

                var assetDependencies = AssetDatabase.GetDependencies(AssetDatabase.GUIDToAssetPath(assetGuids[i]), false);
                foreach (var assetDependency in assetDependencies)
                {
                    if (reverseDependencies.ContainsKey(AssetDatabase.AssetPathToGUID(assetDependency, AssetPathToGUIDOptions.OnlyExistingAssets)) && AssetDatabase.AssetPathToGUID(assetDependency, AssetPathToGUIDOptions.OnlyExistingAssets) != assetGuids[i])
                    {
                        reverseDependencies[AssetDatabase.AssetPathToGUID(assetDependency, AssetPathToGUIDOptions.OnlyExistingAssets)].Add(assetGuids[i]);
                    }
                }
            }
        }
    }

    public class AssetData
    {
        public static AssetData Create(string path, int referencesCount, string warning)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            string typeName;

            if (type != null)
            {
                typeName = type.ToString();
                typeName = typeName.Replace("UnityEngine.", string.Empty);
                typeName = typeName.Replace("UnityEditor.", string.Empty);
            }
            else
            {
                typeName = "Unknown Type";
            }

            var isAddressable = CommonUtilities.IsAssetAddressable(path);

            var fileInfo = new FileInfo(path);
            var bytesSize = fileInfo.Length;
            return new AssetData(path, type, typeName, bytesSize,
                CommonUtilities.GetReadableSize(bytesSize), isAddressable, referencesCount, warning);
        }

        private AssetData(string path, Type type, string typeName, long bytesSize,
            string readableSize, bool addressable, int referencesCount, string warning)
        {
            Path = path;
            ShortPath = Path.Replace("Assets/", string.Empty);
            Type = type;
            TypeName = typeName;
            BytesSize = bytesSize;
            ReadableSize = readableSize;
            IsAddressable = addressable;
            ReferencesCount = referencesCount;
            Warning = warning;
        }

        public string Path { get; }
        public string ShortPath { get; }
        public Type Type { get; }
        public string TypeName { get; }
        public long BytesSize { get; }
        public string ReadableSize { get; }
        public bool IsAddressable { get; }
        public int ReferencesCount { get; }
        public string Warning { get; }
        public bool ValidType => Type != null;
        public bool Foldout { get; set; }
    }

    public static class GUIUtilities
    {
        private static void HorizontalLine(
            int marginTop,
            int marginBottom,
            int height,
            Color color
        )
        {
            EditorGUILayout.BeginHorizontal();
            var rect = EditorGUILayout.GetControlRect(
                false,
                height,
                new GUIStyle { margin = new RectOffset(0, 0, marginTop, marginBottom) }
            );

            EditorGUI.DrawRect(rect, color);
            EditorGUILayout.EndHorizontal();
        }

        public static void HorizontalLine(
            int marginTop = 5,
            int marginBottom = 5,
            int height = 2
        )
        {
            HorizontalLine(marginTop, marginBottom, height, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }

    public static class CommonUtilities
    {
        public static string GetReadableSize(long bytesSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytesSize;
            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public static bool IsAssetAddressable(string assetPath)
        {
#if HUNT_ADDRESSABLES
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));
            return entry != null;
#else
            return false;
#endif
        }
    }
}