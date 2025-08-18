using PetoonsStudio.PSEngine.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PetoonsStudio.PSEngine.Tools
{
    public class AudioFormatter : EditorWindow
    {
        private static EditorWindow m_Window = null;

        private bool m_ChangeLoadType = false;
        private AudioClipLoadType m_BaseLoadType = AudioClipLoadType.DecompressOnLoad;
        private bool m_ChangeFormat = false;
        private AudioCompressionFormat m_BaseFormat = AudioCompressionFormat.Vorbis;
        private bool m_ChangeQuality = false;
        private float m_BaseQuality = 0.7f;         //Percentage -> 70%

        private Platform m_OverwriteOnPlatforms = 0;

        [MenuItem("Window/Petoons Studio/PSEngine/Optimization/Audio Formater", priority = ToolsUtils.OPTIMIZATION_CATEGORY)]
        private static void Init()
        {
            m_Window = GetWindow<AudioFormatter>();
        }

        private void OnGUI()
        {
            DrawBaseEditor();
        }

        private void DrawBaseEditor()
        {
            EditorGUILayout.Space(10);

            m_ChangeLoadType = EditorGUILayout.Toggle("Change load type?", m_ChangeLoadType);
            GUI.enabled = m_ChangeLoadType;
            m_BaseLoadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("Load type", m_BaseLoadType);
            GUI.enabled = true;

            EditorGUILayout.Space(20);

            m_ChangeFormat = EditorGUILayout.Toggle("Change format?", m_ChangeFormat);
            GUI.enabled = m_ChangeFormat;
            m_BaseFormat = (AudioCompressionFormat)EditorGUILayout.EnumPopup("Format", m_BaseFormat);
            GUI.enabled = true;

            EditorGUILayout.Space(20);

            m_ChangeQuality = EditorGUILayout.Toggle("Change quality?", m_ChangeQuality);
            GUI.enabled = m_ChangeQuality;
            m_BaseQuality = EditorGUILayout.FloatField("Quality", m_BaseQuality);
            GUI.enabled = true;

            EditorGUILayout.Space(30);

            EditorGUILayout.LabelField("Platforms", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Overwrite for selected platforms");
            m_OverwriteOnPlatforms = (Platform)EditorGUILayout.EnumFlagsField("Platforms", m_OverwriteOnPlatforms);
            EditorGUILayout.LabelField("     *Select Nothing or Everything to change the default only");

            EditorGUILayout.Space(30);

            EditorGUILayout.BeginVertical("helpBox");
            EditorGUILayout.LabelField("  Summary", EditorStyles.boldLabel);
            if (m_ChangeLoadType)
                EditorGUILayout.LabelField($"Audio load type will be changed to {m_BaseLoadType}");
            if (m_ChangeFormat)
                EditorGUILayout.LabelField($"Audio format will be changed to {m_BaseFormat}");
            if (m_ChangeQuality)
                EditorGUILayout.LabelField($"Audio quality will be changed to {m_BaseQuality * 100}%");

            EditorGUILayout.Space(15);
            ShowPlatformSummaryInfo();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Apply"))
            {
                FormatAudios(GetPathsAtSelection());
            }
        }

        private void ShowPlatformSummaryInfo()
        {
            if (IsDefault(m_OverwriteOnPlatforms))
            {
                EditorGUILayout.LabelField("Apply on default");
            }
            else
            {
                var platforms = m_OverwriteOnPlatforms.ToString();
                EditorGUILayout.LabelField($"Apply on {platforms}");
            }
        }

        private bool IsDefault(Platform platform)
        {
            // -1 is everything selected
            if ((int)platform <= 0)
                return true;

            return false;
        }

        private void FormatAudios(List<string> paths)
        {
            foreach (var audioPath in paths)
            {
                var importer = AssetImporter.GetAtPath(audioPath) as AudioImporter;

                if (importer == null) continue;

                var settings = importer.defaultSampleSettings;
                if (m_ChangeLoadType)
                    settings.loadType = m_BaseLoadType;
                if (m_ChangeFormat)
                    settings.compressionFormat = m_BaseFormat;
                if (m_ChangeQuality)
                    settings.quality = m_BaseQuality;


                if (IsDefault(m_OverwriteOnPlatforms))
                {
                    importer.defaultSampleSettings = settings;
                }
                else
                {
                    if (m_OverwriteOnPlatforms.HasFlag(Platform.Ps4))
                        importer.SetOverrideSampleSettings(RuntimePlatform.PS4.ToString(), settings);
                    if (m_OverwriteOnPlatforms.HasFlag(Platform.Ps5))
                        importer.SetOverrideSampleSettings(RuntimePlatform.PS5.ToString(), settings);
                    if (m_OverwriteOnPlatforms.HasFlag(Platform.XboxOne))
                        importer.SetOverrideSampleSettings(RuntimePlatform.GameCoreXboxOne.ToString(), settings);
                    if (m_OverwriteOnPlatforms.HasFlag(Platform.XboxSeries))
                        importer.SetOverrideSampleSettings(RuntimePlatform.GameCoreXboxSeries.ToString(), settings);
                    if (m_OverwriteOnPlatforms.HasFlag(Platform.Switch))
                        importer.SetOverrideSampleSettings(RuntimePlatform.Switch.ToString(), settings);
                    if (m_OverwriteOnPlatforms.HasFlag(Platform.Steam) || m_OverwriteOnPlatforms.HasFlag(Platform.Epic) ||
                        m_OverwriteOnPlatforms.HasFlag(Platform.Gog) || m_OverwriteOnPlatforms.HasFlag(Platform.WindowsStore) ||
                        m_OverwriteOnPlatforms.HasFlag(Platform.Standalone) || m_OverwriteOnPlatforms.HasFlag(Platform.Editor))
                        importer.SetOverrideSampleSettings(RuntimePlatform.WindowsPlayer.ToString(), settings);
                }

                importer.SaveAndReimport();
            }
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
    }
}