using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class FindAsset : EditorWindow
    {
        private string m_GUID;
        private string m_AssetEntry;
        private string[] m_ToolbarTitles = { "GUID", "Entry GUID"};
        private int m_ToolbarSelection = 0;
        private int m_PreviousToolbarSelection = 0;
        private string m_ErrorMesage;

        [MenuItem("Window/Petoons Studio/PSEngine/Editor/Find Asset", priority = ToolsUtils.EDITOR_CATEGORY)]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            FindAsset window = (FindAsset)EditorWindow.GetWindow(typeof(FindAsset));
            window.titleContent = new GUIContent("Find Asset");
            window.minSize = new Vector2(250f, 60f);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            m_ToolbarSelection = GUILayout.Toolbar(m_ToolbarSelection, m_ToolbarTitles);
            GUILayout.EndHorizontal();

            switch (m_ToolbarSelection)
            {
                case 0:
                    FindByGUIDDraw();
                    break;
                case 1:
                    FindByAssetEntryDraw();
                    break;
                default:
                    break;
            }

            ResteCategoryIfNecessary();
            m_PreviousToolbarSelection = m_ToolbarSelection;
        }

        private void ResteCategoryIfNecessary()
        {
            if (m_ToolbarSelection != m_PreviousToolbarSelection)
            {
                m_ErrorMesage = string.Empty;
            }
        }

        private void FindByGUIDDraw()
        {
            GUILayout.Label("GUID to search for:");
            m_GUID = GUILayout.TextField(m_GUID);

            if (GUILayout.Button("Search"))
            {
                if(FindAssetGUID(m_GUID, out Object asset))
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(asset);
                }
                else
                {
                    m_ErrorMesage = $"Asset with GUID: {m_GUID} was not found!";
                }
            }

            if(!string.IsNullOrEmpty(m_ErrorMesage))    
                EditorGUILayout.HelpBox(m_ErrorMesage, MessageType.Error);
        }

        private void FindByAssetEntryDraw()
        {
            GUILayout.Label("Asset Entry GUID of Addressable to search for:");
            m_AssetEntry = GUILayout.TextField(m_AssetEntry);

            if (GUILayout.Button("Search"))
            {
                if (FindAssetEntry(m_AssetEntry, out AddressableAssetEntry entry))
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(entry.MainAsset);
                }
                else
                {
                    m_ErrorMesage = $"Asset with entry GUID: {m_AssetEntry} was not found!";
                }
            }

            if (!string.IsNullOrEmpty(m_ErrorMesage))
                EditorGUILayout.HelpBox(m_ErrorMesage, MessageType.Error);
        }

        private bool FindAssetGUID(string guid, out Object asset)
        {
            asset = null;

            if (string.IsNullOrEmpty(guid)) return false;
            string path = AssetDatabase.GUIDToAssetPath(m_GUID);

            if (string.IsNullOrEmpty(path)) return false;
            asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            return true;
        }

        private bool FindAssetEntry(string GUIDEntry, out AddressableAssetEntry entry)
        {
            if(AddressableAssetSettingsDefaultObject.Settings == null)
            {
                entry = null;
                return false;
            }

            entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(GUIDEntry);
            return entry != null;
        }
    } 
}
