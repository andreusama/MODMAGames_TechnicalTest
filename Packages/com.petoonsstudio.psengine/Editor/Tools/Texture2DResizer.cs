using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PetoonsStudio.PSEngine.Tools
{
    public class Texture2DResizer : EditorWindow
    {
        public enum SizeOptions
        {
            S_4096 = 4096,
            S_2048 = 2048,
            S_1024 = 1024,
            S_512 = 512,
            S_256 = 256,
            S_128 = 128,
            S_64 = 64,
            S_32 = 32,
            S_0 = 0
        }

        public enum PlatformOptions : byte
        {
            None = 0,
            Switch = 1,
            PS4 = 2,
            PS5 = 4,
            XboxOne = 8,
            XboxSeries = 16
        }

        private static EditorWindow m_Window = null;
        private string[] m_ToolbarTitles = { "Resize", "Platform", "Settings", "NPOT Textures" };
        private int m_ToolbarSelection = 0;
        private float m_StartTime = 0;
        private float m_EndTime = 0;

        private SizeOptions m_BaseSize = SizeOptions.S_1024;
        private int m_SelectedTexturesDivider = 2;
        private bool m_DivideNormal = true;
        private bool m_DivideMetallic = true;
        private bool m_DivideRoughness = true;
        private bool m_DivideEmission = true;
        private bool m_DivideAO = true;

        private PlatformOptions m_PlatformOption = PlatformOptions.None;
        private int m_PlatformDivider = 4;
        private SizeOptions m_ExcludeSize = SizeOptions.S_128;
        private TextureImporterFormat m_TextureFormat = TextureImporterFormat.DXT5Crunched;
        private int m_CrunchCompressionRate = 80;

        private int m_PlatformMinResolution = 64;
        private string m_NormalIndicator = "_Normal_tx";
        private string m_MetallicIndicator = "_Metallic_tx";
        private string m_RoughnessIndicator = "_Roughness_tx";
        private string m_EmissionIndicator = "_Emission_tx";
        private string m_AOIndicator = "_AO_tx";

        private List<string> m_TexturesNPOT = new List<string>();
        private Vector2 m_NPOTTexturesScroll;


        [MenuItem("Window/Petoons Studio/PSEngine/Optimization/Texture Utilities", priority = ToolsUtils.OPTIMIZATION_CATEGORY)]
        private static void Init()
        {
            m_Window = GetWindow<Texture2DResizer>();
            m_Window.titleContent = new GUIContent("Texture Utilities");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            m_ToolbarSelection = GUILayout.Toolbar(m_ToolbarSelection, m_ToolbarTitles);
            GUILayout.EndHorizontal();

            switch (m_ToolbarSelection)
            {
                case 0:
                    DrawBaseResizeEditor();
                    break;
                case 1:
                    DrawPlatformResizeEditor();
                    break;
                case 2:
                    DrawSettingsEditor();
                    break;
                case 3:
                    DrawNPOTTextures();
                    break;
            }
        }

        private void DrawNPOTTextures()
        {
            EditorGUILayout.BeginVertical("helpBox");
            EditorGUILayout.LabelField("  Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("List of textures NPOT");
            EditorGUILayout.LabelField("Select the folder or the textures to check and click Refresh List");
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Refresh List"))
            {
                m_TexturesNPOT = GetTextureNPOT(GetPathsAtSelection());
            }
            EditorGUILayout.BeginVertical("Helpbox");

            EditorGUILayout.LabelField("Textures:");
            m_NPOTTexturesScroll = EditorGUILayout.BeginScrollView(m_NPOTTexturesScroll);
            foreach (var texture in m_TexturesNPOT)
            {
                if (GUILayout.Button(texture))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texture);
                    Selection.activeObject = tex;
                    EditorGUIUtility.PingObject(tex);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private List<string> GetTextureNPOT(List<string> textures)
        {
            List<string> result = new List<string>();
            foreach (var texture in textures)
            {
                var importer = TextureImporter.GetAtPath(texture) as TextureImporter;

                if (importer == null) continue;

                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texture);
                if (!Mathf.IsPowerOfTwo(tex.width) || !Mathf.IsPowerOfTwo(tex.height))
                    result.Add(texture);
            }
            return result;
        }

        private void DrawBaseResizeEditor()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Resize base texture to:", EditorStyles.boldLabel);
            m_BaseSize = (SizeOptions)EditorGUILayout.EnumPopup("Size", m_BaseSize);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Additional resize by type:", EditorStyles.boldLabel);
            m_SelectedTexturesDivider = EditorGUILayout.IntField("Divider", m_SelectedTexturesDivider);
            EditorGUILayout.LabelField($"Selected types size: {(int)m_BaseSize / m_SelectedTexturesDivider}");
            if (m_SelectedTexturesDivider <= 0)
                m_SelectedTexturesDivider = 1;

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            m_DivideNormal = EditorGUILayout.Toggle("Normal", m_DivideNormal);
            m_DivideMetallic = EditorGUILayout.Toggle("Metallic", m_DivideMetallic);
            m_DivideRoughness = EditorGUILayout.Toggle("Roughness", m_DivideRoughness);
            m_DivideEmission = EditorGUILayout.Toggle("Emission", m_DivideEmission);
            m_DivideAO = EditorGUILayout.Toggle("AO", m_DivideAO);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(30);

            EditorGUILayout.BeginVertical("helpBox");
            EditorGUILayout.LabelField("  Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"All textures will be resized to {(int)m_BaseSize}");
            if (m_DivideNormal || m_DivideMetallic || m_DivideRoughness || m_DivideEmission || m_DivideAO)
                EditorGUILayout.LabelField($"Textures from selected types will be resized to {(int)m_BaseSize / m_SelectedTexturesDivider}");
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Resize"))
            {
                m_StartTime = Time.time;
                Debug.Log($"Starting at :{m_StartTime}s");
                ResizeSelectedTextures(GetPathsAtSelection());
                m_EndTime = Time.time;
                Debug.Log($"Ending at :{m_EndTime}s");
                Debug.Log($"Resizing took: {m_EndTime - m_StartTime}s");
            }
        }

        private void DrawPlatformResizeEditor()
        {
            EditorGUILayout.LabelField("Select platforms:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            m_PlatformOption = (PlatformOptions)EditorGUILayout.EnumFlagsField(m_PlatformOption);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            m_PlatformDivider = EditorGUILayout.IntField("Divider", m_PlatformDivider);
            if (m_PlatformDivider < 0)
                m_PlatformDivider = 1;

            m_ExcludeSize = (SizeOptions)EditorGUILayout.EnumPopup(new GUIContent("Exclude <= size", "Texture size above which the maximum size change will not be executed."), m_ExcludeSize);
            m_TextureFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture format", m_TextureFormat);
            m_CrunchCompressionRate = EditorGUILayout.IntField("Crunch Compression Rate", m_CrunchCompressionRate);

            EditorGUILayout.Space(30);

            EditorGUILayout.BeginVertical("helpBox");
            EditorGUILayout.LabelField("  Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Settings will be overwritten for selected platforms");
            EditorGUILayout.LabelField($"and size will be default size / {m_PlatformDivider}");
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Resize"))
            {
                m_StartTime = Time.time;
                Debug.Log($"Starting at :{m_StartTime}s");

                UpdatePlatformSpecificImageCompression(GetPathsAtSelection());

                m_EndTime = Time.time;
                Debug.Log($"Ending at :{m_EndTime}s");

                Debug.Log($"Platform resizing took: {m_EndTime - m_StartTime}s");
            }

            EditorGUILayout.BeginVertical("helpBox");
            EditorGUILayout.LabelField("  Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Settings will be reverted for selected platforms");
            EditorGUILayout.LabelField($"and all settings will match default settings");
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Restore"))
            {
                m_StartTime = Time.time;
                Debug.Log($"Starting at :{m_StartTime}s");

                RestorePlatformSpecificImageCompression(GetPathsAtSelection());

                m_EndTime = Time.time;
                Debug.Log($"Ending at :{m_EndTime}s");

                Debug.Log($"Platform restore took: {m_EndTime - m_StartTime}s");
            }
        }

        private void DrawSettingsEditor()
        {
            EditorGUILayout.LabelField("Texture types", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    Texture is of given type if texture name contains exactly:");
            EditorGUI.indentLevel++;
            m_NormalIndicator = EditorGUILayout.TextField("Normal", m_NormalIndicator);
            m_MetallicIndicator = EditorGUILayout.TextField("Metallic", m_MetallicIndicator);
            m_RoughnessIndicator = EditorGUILayout.TextField("Roughness", m_RoughnessIndicator);
            m_EmissionIndicator = EditorGUILayout.TextField("Emission", m_EmissionIndicator);
            m_AOIndicator = EditorGUILayout.TextField("AO", m_AOIndicator);
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Platform settings", EditorStyles.boldLabel);
            m_PlatformMinResolution = EditorGUILayout.IntField("Min resolution", m_PlatformMinResolution);
        }

        private void ResizeSelectedTextures(List<string> paths)
        {
            foreach (var texturePath in paths)
            {
                var importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
                var texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;

                if (importer == null) continue;

                if (m_DivideNormal && texture.name.Contains(m_NormalIndicator))
                    importer.maxTextureSize = (int)m_BaseSize / m_SelectedTexturesDivider;
                else if (m_DivideMetallic && texture.name.Contains(m_MetallicIndicator))
                    importer.maxTextureSize = (int)m_BaseSize / m_SelectedTexturesDivider;
                else if (m_DivideRoughness && texture.name.Contains(m_RoughnessIndicator))
                    importer.maxTextureSize = (int)m_BaseSize / m_SelectedTexturesDivider;
                else if (m_DivideEmission && texture.name.Contains(m_EmissionIndicator))
                    importer.maxTextureSize = (int)m_BaseSize / m_SelectedTexturesDivider;
                else if (m_DivideAO && texture.name.Contains(m_AOIndicator))
                    importer.maxTextureSize = (int)m_BaseSize / m_SelectedTexturesDivider;
                else
                    importer.maxTextureSize = (int)m_BaseSize;

                importer.SaveAndReimport();
            }
        }

        private void UpdatePlatformSpecificImageCompression(List<string> paths)
        {
            foreach (var texturePath in paths)
            {
                var importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;

                if (importer == null) continue;

                var defaultSettings = importer.GetDefaultPlatformTextureSettings();
                var changed = false;

                Action<TextureImporterPlatformSettings> SetPlatformSettings = (platSettings) =>
                {
                    if (platSettings.maxTextureSize >= (int)m_ExcludeSize)
                        platSettings.maxTextureSize = GetWantedResolution(defaultSettings.maxTextureSize);
                    platSettings.overridden = true;
                    platSettings.crunchedCompression = true;
                    platSettings.format = m_TextureFormat;
                    platSettings.compressionQuality = m_CrunchCompressionRate;

                    changed = true;
                    importer.SetPlatformTextureSettings(platSettings);
                };

                if (m_PlatformOption.HasFlag(PlatformOptions.Switch))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.Switch.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.PS4))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.PS4.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.PS5))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.PS5.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.XboxOne))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.GameCoreXboxOne.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.XboxSeries))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.GameCoreXboxSeries.ToString()));

                if (changed)
                    importer.SaveAndReimport();
            }
        }

        private void RestorePlatformSpecificImageCompression(List<string> paths)
        {
            foreach (var texturePath in paths)
            {
                var importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
                var texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;

                if (importer == null) continue;

                var defaultSettings = importer.GetDefaultPlatformTextureSettings();
                var changed = false;

                Action<TextureImporterPlatformSettings> SetPlatformSettings = (platSettings) =>
                {
                    platSettings.maxTextureSize = defaultSettings.maxTextureSize;
                    platSettings.overridden = false;
                    platSettings.crunchedCompression = false;
                    platSettings.format = defaultSettings.format;
                    platSettings.compressionQuality = 100;

                    changed = true;
                    importer.SetPlatformTextureSettings(platSettings);
                };

                if (m_PlatformOption.HasFlag(PlatformOptions.Switch))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.Switch.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.PS4))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.PS4.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.PS5))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.PS5.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.XboxOne))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.GameCoreXboxOne.ToString()));
                if (m_PlatformOption.HasFlag(PlatformOptions.XboxSeries))
                    SetPlatformSettings(importer.GetPlatformTextureSettings(RuntimePlatform.GameCoreXboxSeries.ToString()));

                if (changed)
                    importer.SaveAndReimport();
            }
        }

        private int GetWantedResolution(int defaultTextureSize)
        {
            int newResolution = defaultTextureSize / m_PlatformDivider;
            newResolution = Mathf.ClosestPowerOfTwo(newResolution);
            newResolution = newResolution < m_PlatformMinResolution ? m_PlatformMinResolution : newResolution;
            return newResolution;
        }

        private List<string> GetPathsAtSelection()
        {
            List<string> returnList = new List<string>();
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj); // relative path
                List<string> paths = GetPathsAtPath(selectionPath);
                returnList.AddRange(paths);
            }
            return returnList;
        }

        private List<string> GetPathsAtPath(string path)
        {
            List<string> fileEntries = new List<string>();
            string[] directoriesEntries;

            if (Directory.Exists(path))
            {
                directoriesEntries = Directory.GetDirectories(path);
                if (directoriesEntries != null && directoriesEntries.Length > 0)
                {
                    foreach (string directoryEntry in directoriesEntries)
                    {
                        fileEntries.AddRange(GetPathsAtPath(directoryEntry));
                    }
                }
                fileEntries.AddRange(Directory.GetFiles(path)
                    .Where(name => !name.EndsWith(".meta")).ToArray<string>());
            }
            else
            {
                fileEntries.Add(path);
            }

            return fileEntries;
        }

        private List<Object> GetAllObjectsAtSelection()
        {
            List<Object> returnList = new List<Object>();
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj); // relative path
                Object[] assets = GetAssetsAtPath<Object>(selectionPath);
                returnList.AddRange(assets);
            }

            return returnList;
        }

        private string SolveSeparatorsConvention(string entryPath)
        {
            int index = entryPath.LastIndexOf("\\");
            string path = entryPath;
            path = path.Remove(index);

            if (index > 0)
            {
                path += "/";
            }
            path += entryPath.Substring(index + 1);

            return path;
        }

        private T[] GetAssetsAtPath<T>(string path)
        {
            ArrayList Results = new ArrayList();

            List<string> fileEntries = GetPathsAtPath(path);

            foreach (string fileName in fileEntries)
            {
                string localPath = fileName;

                localPath = SolveSeparatorsConvention(localPath);

                Object tempObject = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (tempObject != null)
                    Results.Add(tempObject);
            }

            T[] result = new T[Results.Count];
            for (int i = 0; i < Results.Count; i++)
                result[i] = (T)Results[i];

            return result;
        }
    }
}