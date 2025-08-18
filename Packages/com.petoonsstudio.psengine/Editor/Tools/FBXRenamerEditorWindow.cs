using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class FBXRenamerEditorWindow : EditorWindow
    {
        private static FBXRenamerEditorWindow m_Window;

        private const int WIN_WIDTH = 400;
        private const int WIN_HEIGHT = 100;

        private List<string> m_AllFiles;

        private string m_SearchFbxPath = "";

        [MenuItem("Window/Petoons Studio/PSEngine/Editor/FBX Clip Renamer", priority = ToolsUtils.EDITOR_CATEGORY)]
        private static void ShowEditor()
        {
            m_Window = (FBXRenamerEditorWindow)GetWindow(typeof(FBXRenamerEditorWindow), false, "FBX Clip Renamer");

            m_Window.position = new Rect((Screen.currentResolution.width - WIN_WIDTH) / 2f, (Screen.currentResolution.height - WIN_HEIGHT) / 2f, WIN_WIDTH, WIN_HEIGHT);
            m_Window.maxSize = new Vector2(WIN_WIDTH, WIN_HEIGHT);
            m_Window.minSize = m_Window.maxSize;
        }

        private void OnGUI()
        {
            GUILayout.Space(30);
            GUILayout.Label("Path relative to Assets folder. Example(/Animations/fbxfiles/)");
            m_SearchFbxPath = GUI.TextField(new Rect(5, 50, WIN_WIDTH - 10, 21), m_SearchFbxPath, 200);

            GUILayout.Space(30);
            GUI.enabled = true;

            if (GUILayout.Button("Rename")) RenameAllClips();
        }

        private void RenameAllClips()
        {
            DirSearch();

            if (m_AllFiles.Count < 1) return;

            foreach (string file in m_AllFiles)
            {
                string asset = file.Substring(file.IndexOf("Assets", StringComparison.Ordinal));
                RenameAndImport(AssetImporter.GetAtPath(asset) as ModelImporter, Path.GetFileNameWithoutExtension(Path.GetFileName(file)));
            }
        }

        private static void RenameAndImport(ModelImporter asset, string newClipName)
        {
            ModelImporterClipAnimation[] renamedAsset = asset.defaultClipAnimations;
            foreach (ModelImporterClipAnimation clip in renamedAsset) clip.name = newClipName;

            asset.clipAnimations = renamedAsset;
            asset.SaveAndReimport();
        }

        private void DirSearch()
        {
            m_AllFiles = new List<string>();
            string path = (Application.dataPath + m_SearchFbxPath).Replace("/Assets/Assets/", "/Assets/")
                                                                  .Replace("/AssetsAssets/", "/Assets/");

            if (Directory.Exists(path)) m_AllFiles.AddRange(Directory.GetFiles(path, "*.fbx", SearchOption.AllDirectories));
            else Debug.Log("<color=\"cyan\">Directory does not exist</color>");
        }
    }
}
