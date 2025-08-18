using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class ReplacePrefabTool : EditorWindow
    {
        private GameObject m_Prefab;
        
        private GUIStyle m_TitleStyle;

        private const string DESCRIPTION =
            "All selected GameObjects are replaced with the specified prefab. " +
            "\nParent transform, position, local rotation & local scale are always mantained.";

        [MenuItem("Window/Petoons Studio/PSEngine/Editor/Replace Prefab Tool", priority = ToolsUtils.EDITOR_CATEGORY)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            ReplacePrefabTool window = (ReplacePrefabTool)EditorWindow.GetWindow(typeof(ReplacePrefabTool));
            window.titleContent = new GUIContent("Replace Prefab Tool");
            window.minSize = new Vector2(250f, 150f);
            window.Show();
        }

        private void OnEnable()
        {
            m_TitleStyle = new GUIStyle();
            m_TitleStyle.normal.textColor = Color.gray;
            m_TitleStyle.fontSize = 26;
            m_TitleStyle.alignment = TextAnchor.MiddleCenter;
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Replace Prefab Tool"), m_TitleStyle, GUILayout.MinHeight(30));

            EditorGUILayout.HelpBox(DESCRIPTION, MessageType.Info);

            m_Prefab = EditorGUILayout.ObjectField(m_Prefab, typeof(GameObject), false) as GameObject;

            if (GUILayout.Button("Replace Selected Objects w/ Prefab"))
            {
                ReplaceSelectedObjectsWithPrefab();
            }
        }

        private void ReplaceSelectedObjectsWithPrefab()
        {
            if (!m_Prefab) return;

            List<GameObject> newObjects = new List<GameObject>();
            Queue<GameObject> oldObjects = new Queue<GameObject>(Selection.gameObjects);

            int undoID = Undo.GetCurrentGroup();

            while (oldObjects.Count > 0)
            {
                var obj = oldObjects.Dequeue();
                var newObj = ReplaceObjectWithPrefab(obj);
                newObjects.Add(newObj);
                Undo.RegisterCreatedObjectUndo(newObj, $"Replaced old object with {newObj.name}");
            }

            Undo.CollapseUndoOperations(undoID);

            Selection.objects = newObjects.ToArray();
        }

        private GameObject ReplaceObjectWithPrefab(GameObject oldObject)
        {
            int hierarchyIndex = oldObject.transform.GetSiblingIndex();

            GameObject newObject = PrefabUtility.InstantiatePrefab(m_Prefab, oldObject.transform.parent) as GameObject;
            newObject.transform.position = oldObject.transform.position;
            newObject.transform.localRotation = oldObject.transform.localRotation;
            newObject.transform.localScale = oldObject.transform.localScale;

            Undo.DestroyObjectImmediate(oldObject);
            newObject.transform.SetSiblingIndex(hierarchyIndex);

            return newObject;
        }
    } 
}
