using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class TextureArrayGeneratorTool : EditorWindow
    {
        private List<Texture2D> m_SingleTextures;

        private GUIStyle m_ListButtonStyle;
        private Vector2 m_ScrollPos;

        [MenuItem("Window/Petoons Studio/PSEngine/Editor/Texture Array Generator", priority = ToolsUtils.EDITOR_CATEGORY)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            TextureArrayGeneratorTool window = (TextureArrayGeneratorTool)EditorWindow.GetWindow(typeof(TextureArrayGeneratorTool));
            window.titleContent = new GUIContent("Texture Array Generator");
            window.Show();

            window.Setup();
        }

        public void Setup()
        {
            m_SingleTextures = new List<Texture2D>();
            m_SingleTextures.Add(null);

            m_ListButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontSize = 26
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawSingleTexturesList();
            DrawSingleTexturesListButtons();

            if (m_SingleTextures.Count > 0 && GUILayout.Button("Generate Texture Array Asset"))
            {
                Texture2DArray texArray = GenerateTextureArray();
                string assetPath = EditorUtility.SaveFilePanelInProject("Save Texture Array Asset", "Texture2DArray", "asset", "Message");
                AssetDatabase.CreateAsset(texArray, assetPath);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSingleTexturesList()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, EditorStyles.helpBox);

            for (int i = 0; i < m_SingleTextures.Count; i++)
            {
                m_SingleTextures[i] = (Texture2D)EditorGUILayout.ObjectField(m_SingleTextures[i], typeof(Texture2D), false);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSingleTexturesListButtons()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(" + ", m_ListButtonStyle, GUILayout.MaxWidth(70)))
            {
                m_SingleTextures.Add(null);
            }

            if (GUILayout.Button(" - ", m_ListButtonStyle, GUILayout.MaxWidth(70)))
            {
                if (m_SingleTextures.Count > 0)
                    m_SingleTextures.RemoveAt(m_SingleTextures.Count - 1);
            }

            EditorGUILayout.EndHorizontal();
        }

        private Texture2DArray GenerateTextureArray()
        {
            Texture2DArray texture2DArray = new Texture2DArray(m_SingleTextures[0].width,
                                                m_SingleTextures[0].height,
                                                m_SingleTextures.Count,
                                                TextureFormat.RGBA32,
                                                true,
                                                false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat
            };

            for (int i = 0; i < m_SingleTextures.Count; i++)
            {
                texture2DArray.SetPixels(m_SingleTextures[i].GetPixels(0),
                    i, 0);
            }

            texture2DArray.Apply();

            return texture2DArray;
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