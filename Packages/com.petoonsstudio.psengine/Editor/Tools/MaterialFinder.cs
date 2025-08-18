using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class MaterialFinder : EditorWindow
    {
        private string[] m_ToolbarTitles = { "Shader", "Missing" };
        private int m_ToolbarSelection = 0;
        private List<Material> m_ProjectMaterials;
        private List<Material> m_FilteredMaterials;
        private List<GameObject> m_MissingMaterials = new List<GameObject>();
        private Shader m_SearchedShader;
        private Vector2 m_ScrollPos;
        private Shader m_ReplaceShader;

        [MenuItem("Window/Petoons Studio/PSEngine/Editor/Find Material", priority = ToolsUtils.EDITOR_CATEGORY)]
        public static MaterialFinder OpenEditorWindow()
        {
            // Get existing open window or if none, make a new one:
            MaterialFinder window = (MaterialFinder)EditorWindow.GetWindow(typeof(MaterialFinder));
            window.titleContent = new GUIContent("Material Finder");
            window.minSize = new Vector2(485f, window.minSize.y);
            window.Show();
            window.Setup();
            return window;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            m_ToolbarSelection = GUILayout.Toolbar(m_ToolbarSelection, m_ToolbarTitles);
            GUILayout.EndHorizontal();

            switch (m_ToolbarSelection)
            {
                case 0:
                    DrawShaderFind();
                    break;
                case 1:
                    DrawMissingFind();
                    break;
            }
        }

        private void DrawMissingFind()
        {
            DrawMissingMaterialsList();

            if (GUILayout.Button("Find Missing Materials"))
                FindMissingMaterials();
        }

        private void FindMissingMaterials()
        {
            m_MissingMaterials.Clear();
            GameObject[] gameObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject go in gameObjects)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] == null)
                            m_MissingMaterials.Add(go);
                    }
                }
            }
        }

        private void DrawMissingMaterialsList()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            for (int i = 0; i < m_MissingMaterials.Count; i++)
                DrawMissingMaterial(m_MissingMaterials[i]);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"Materials found: {m_MissingMaterials.Count}");

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMissingMaterial(GameObject go)
        {
            if (GUILayout.Button(go.name, EditorStyles.toolbarButton))
            {
                EditorGUIUtility.PingObject(go);
                Selection.objects = new Object[] { go };
            }
        }

        private void DrawShaderFind()
        {
            var searchShader = DrawShaderField(m_SearchedShader, "Filter by Shader");
            if (searchShader != m_SearchedShader)
            {
                m_SearchedShader = searchShader;
                m_FilteredMaterials = FilterMaterialsByShader(m_ProjectMaterials, m_SearchedShader);
            }

            DrawMaterialsList();
            DrawToolbar();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (GUILayout.Button("Refresh Materials"))
                m_ProjectMaterials = FetchAllMaterials();

            m_ReplaceShader = DrawShaderField(m_ReplaceShader, "Replace shader");

            if (m_ReplaceShader != null && GUILayout.Button("Replace Shader in All"))
            {
                ReplaceShaderOnFilteredMaterials();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ReplaceShaderOnFilteredMaterials()
        {
            for (int i = 0; i < m_FilteredMaterials.Count; i++)
            {
                Undo.RecordObject(m_FilteredMaterials[i], "Replace Shader");
                m_FilteredMaterials[i].shader = m_ReplaceShader;
            }
        }

        private void DrawMaterialsList()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0; i < m_FilteredMaterials.Count; i++)
            {
                DrawMaterial(m_FilteredMaterials[i]);
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"Materials found: {m_FilteredMaterials.Count}");

            EditorGUILayout.EndHorizontal();
        }

        private Shader DrawShaderField(Shader shader, string header)
        {
            EditorGUILayout.BeginVertical();
            if (!string.IsNullOrEmpty(header))
            {
                GUILayout.Label(header, EditorStyles.miniLabel);
            }
            shader = EditorGUILayout.ObjectField(shader, typeof(Shader), false) as Shader;
            EditorGUILayout.EndVertical();
            return shader;
        }

        private void DrawMaterial(Material material)
        {
            if (GUILayout.Button(material.name, EditorStyles.toolbarButton))
            {
                EditorGUIUtility.PingObject(material);
                Selection.objects = new Object[] { material };
            }
        }

        public void Setup()
        {
            m_ProjectMaterials = FetchAllMaterials();
            m_FilteredMaterials = new List<Material>();
        }

        private List<Material> FetchAllMaterials()
        {
            string[] guid = AssetDatabase.FindAssets("t:" + nameof(Material));
            List<Material> materials = new List<Material>(guid.Length);

            for (int i = 0; i < guid.Length; i++)
            {
                materials.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid[i]), typeof(Material)) as Material);
            }

            return materials;
        }

        private List<Material> FilterMaterialsByShader(List<Material> materials, Shader shader)
        {
            List<Material> filteredMaterials = new List<Material>();

            for (int i = 0; i < materials.Count; i++)
            {
                if (materials[i].shader == shader)
                {
                    filteredMaterials.Add(materials[i]);
                }
            }

            return filteredMaterials;
        }
    }
}
