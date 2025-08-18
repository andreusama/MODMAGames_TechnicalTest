using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PetoonsStudio.PSEngine.Framework;
using UnityEditor.SceneManagement;

namespace PetoonsStudio.PSEngine
{
    [CustomEditor(typeof(SceneGroup), true)]
    public class SceneGroupEditor : Editor
    {
        private SceneGroup m_SceneGroup;

        private void OnEnable()
        {
            m_SceneGroup = (SceneGroup)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();  //Draw general info

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Load Scene Group"))
            {
                if (m_SceneGroup.Scenes == null || m_SceneGroup.Scenes.Length == 0)
                    return;

                bool singleSceneLoaded = false;

                for (int i = 0; i < m_SceneGroup.Scenes.Length; i++)
                {
                    if (m_SceneGroup.Scenes[i].editorAsset != null)
                    {
                        string scenePath = AssetDatabase.GetAssetPath(m_SceneGroup.Scenes[i].editorAsset.GetInstanceID());

                        if (!singleSceneLoaded)
                        {
                            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                            singleSceneLoaded = true;
                        }
                        else
                        {
                            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                        }
                    }
                }
            }
        }
    }
}
