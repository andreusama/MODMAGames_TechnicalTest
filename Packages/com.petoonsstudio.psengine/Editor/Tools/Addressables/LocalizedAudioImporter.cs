using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace PetoonsStudio.PSEngine.Tools
{
    public class LocalizedAudioImporter : EditorWindow
    {
        private class Locale
        {
            public AssetTable Table;
            public int EntriesCount;
            public int ValuesCount;

            public Locale(AssetTable table)
            {
                Table = table;
                UpdateInfo();
            }

            public void UpdateInfo()
            {
                EntriesCount = Table.SharedData.Entries.Count;
                ValuesCount = Table.Values.Count((x) => !x.IsEmpty);
            }
        }

        private AssetTableCollection m_TableCollection;
        private Dictionary<string, Locale> m_Locales;
        private enum Mode { AUTO, FORCE_LOCALE }

        private string m_AudioClipsPath;
        private bool m_AddEntryIfMissing = true;
        private Vector2 m_ScrollPos;
        private int m_CurrentLocale;
        private Mode m_CurrentMode;

        private string DefaultPath => Application.dataPath + "/Audio";

        [MenuItem("Window/Petoons Studio/PSEngine/Localization/Localized Audio Importer", priority = ToolsUtils.LOCALIZATION_CATEGORY)]
        public static void OpenImporter()
        {
            var m_Window = (LocalizedAudioImporter)GetWindow(typeof(LocalizedAudioImporter), false, "Localized Audio Importer");
        }

        [MenuItem("Assets/Petoons Studio/PSEngine/Tools/Import Localized Audio Clips")]
        public static void OpenImporterWithSelectedTable()
        {
            if (Selection.activeObject is AssetTableCollection tableCollection)
            {
                var window = (LocalizedAudioImporter)GetWindow(typeof(LocalizedAudioImporter), false, "Localized Audio Importer");
                window.OpenCollection(tableCollection);
            }
            else
            {
                Debug.LogWarning($"In order to import localized audio clips, please first select the target TableCollection. (Selected was {Selection.activeObject.GetType()})");
            }
        }

        public void OpenCollection(AssetTableCollection tableCollection)
        {
            m_TableCollection = tableCollection;
            m_Locales = new Dictionary<string, Locale>();

            foreach (var table in m_TableCollection.AssetTables)
            {
                m_Locales.Add(table.LocaleIdentifier.Code.ToLower(), new Locale(table));
            }
        }

        private void OnGUI()
        {
            if (DrawAssetTableCollectionPicker())
            {
                DrawAudioClipFolderPicker();
                DrawCurrentModeSelector();
                DrawImportOptions();
                DrawLocaleInfos();
            }
            else
            {
                EditorGUILayout.HelpBox("Please select a target AssetTableCollection into which the clips will be imported.", MessageType.Warning);
            }
        }

        private bool DrawAssetTableCollectionPicker()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Asset Table Collection:");
            var newCollection = (AssetTableCollection)EditorGUILayout.ObjectField(m_TableCollection, typeof(AssetTableCollection), false);

            if (m_TableCollection != newCollection)
            {
                OpenCollection(newCollection);
            }
            else if (m_Locales == null && m_TableCollection != null) // Reopen after recompile.
            {
                OpenCollection(m_TableCollection);
            }

            EditorGUILayout.EndVertical();

            return m_TableCollection != null;
        }

        private void DrawAudioClipFolderPicker()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Audio Clips Path:");
            m_AudioClipsPath = EditorGUILayout.TextField(m_AudioClipsPath);

            if (GUILayout.Button("Select Folder"))
            {
                m_AudioClipsPath = AbsoluteToRelativePath(EditorUtility.OpenFolderPanel("Select the folder that contains the localized audio clips", DefaultPath, ""));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCurrentModeSelector()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_CurrentMode = (Mode)EditorGUILayout.EnumPopup("Import Mode Selector:", m_CurrentMode);

            if (m_CurrentMode == Mode.FORCE_LOCALE)
            {
                DrawLocaleSelector();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawLocaleSelector()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Locale Selector:");
            m_CurrentLocale = EditorGUILayout.Popup(m_CurrentLocale, m_Locales.Keys.ToArray());
            EditorGUILayout.EndVertical();
        }

        private void DrawImportOptions()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (GUILayout.Button("Import"))
            {
                ImportAudioClips();

                foreach (var locale in m_Locales)
                    locale.Value.UpdateInfo();
            }

            GUILayout.Space(20);

            m_AddEntryIfMissing = GUILayout.Toggle(m_AddEntryIfMissing, "   Add entry if missing", GUILayout.MaxWidth(160));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLocaleInfos()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, EditorStyles.helpBox);

            foreach (var locale in m_Locales)
            {
                DrawLocaleInfo(locale.Value);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawLocaleInfo(Locale locale)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);

            GUILayout.Space(20);
            GUILayout.Label($"{locale.Table.LocaleIdentifier.Code} - {locale.Table.LocaleIdentifier.CultureInfo.DisplayName}", EditorStyles.boldLabel);

            GUIStyle style = new GUIStyle(EditorStyles.label);

            if (locale.ValuesCount == 0)
            {
                style.normal.textColor = Color.red;
                style.hover.textColor = Color.red;
            }
            else if (locale.ValuesCount < locale.EntriesCount)
            {
                style.normal.textColor = Color.yellow;
                style.hover.textColor = Color.yellow;
            }
            else
            {
                style.normal.textColor = Color.green;
                style.hover.textColor = Color.green;
            }

            GUILayout.Label($"({locale.ValuesCount}/{locale.EntriesCount})", style, GUILayout.MaxWidth(200));
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
        }

        private void ImportAudioClips()
        {
            var audioClips = FetchAudioClips(out var guids);

            int importedCount = 0;
            int addedCount = 0;

            for (int i = 0; i < audioClips.Length; i++)
            {
                string clipName = audioClips[i].name;
                string localeIdentifier;
                if (m_CurrentMode == Mode.AUTO)
                {
                    ParseClipName(audioClips[i], out clipName, out localeIdentifier);
                }
                else
                {
                    localeIdentifier = m_Locales.Keys.ToArray()[m_CurrentLocale];
                }

                if (m_Locales.TryGetValue(localeIdentifier, out Locale locale))
                {
                    if (!m_TableCollection.SharedData.Contains(clipName))
                    {
                        if (m_AddEntryIfMissing)
                        {
                            m_TableCollection.SharedData.AddKey(clipName);
                            EditorUtility.SetDirty(m_TableCollection.SharedData);
                            addedCount++;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    AddAssetToAddressables(audioClips[i]);
                    locale.Table.AddEntry(clipName, guids[i]);
                    EditorUtility.SetDirty(locale.Table);
                    importedCount++;
                }
                else
                {
                    Debug.LogError($"Was unable to find locale index ({localeIdentifier}) in table collection.");
                }
            }

            EditorUtility.DisplayDialog("Import finished", $"Imported {importedCount} audio clips, of which {addedCount} were new.", "Noice");

            if (importedCount > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void AddAssetToAddressables(UnityEngine.Object asset)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            settings.CreateAssetReference(assetGUID);
        }

        private AudioClip[] FetchAudioClips(out string[] guids)
        {
            guids = AssetDatabase.FindAssets($"t:{nameof(AudioClip)}", new[] { m_AudioClipsPath });

            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            AudioClip[] clips = new AudioClip[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                clips[i] = (AudioClip)AssetDatabase.LoadAssetAtPath(paths[i], typeof(AudioClip));
            }

            return clips;
        }

        private void ParseClipName(AudioClip clip, out string key, out string localeIdentifier)
        {
            int idStartIndex = clip.name.LastIndexOf("_");

            if (idStartIndex < 0)
            {
                key = string.Empty;
                localeIdentifier = string.Empty;

                Debug.LogWarning($"Audio Clip has invalid name structure: it's missing the locale identifier (ex. _en). ({clip.name})");
                return;
            }

            key = clip.name.Substring(0, idStartIndex);
            localeIdentifier = clip.name.Substring(idStartIndex + 1);
            localeIdentifier = localeIdentifier.ToLower();
        }

        private string AbsoluteToRelativePath(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                return "Assets" + path.Substring(Application.dataPath.Length);
            }

            return path;
        }
    }
}