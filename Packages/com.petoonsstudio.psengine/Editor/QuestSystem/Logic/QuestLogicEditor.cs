using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomEditor(typeof(QuestLogic), true)]
    public class QuestLogicEditor : Editor
    {
        SerializedProperty questData;
        SerializedProperty questSteps;

        void OnEnable()
        {
            questSteps = serializedObject.FindProperty("Steps");
            questData = serializedObject.FindProperty("m_QuestID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var t = (target as QuestLogic);

            EditorGUILayout.PropertyField(questData);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));

            for (int i = 0; i < t.Steps.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(UnityEngine.GUI.skin.box);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(questSteps.GetArrayElementAtIndex(i));
                EditorGUI.indentLevel--;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            DrawPropertiesExcluding(serializedObject, "m_QuestID", "m_Script", "Steps");

            serializedObject.ApplyModifiedProperties();
        }
    }
}