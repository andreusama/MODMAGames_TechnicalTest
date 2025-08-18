using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomPropertyDrawer(typeof(QuestStepLogic))]
    public class QuestStepLogicPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty steps = property.FindPropertyRelative(nameof(QuestStepLogic.Step)).FindPropertyRelative(nameof(QuestStepLogic.Step.GUID));
            SerializedProperty description = property.FindPropertyRelative(nameof(QuestStepLogic.Step)).FindPropertyRelative(nameof(QuestStepLogic.Step.Description));

            return EditorGUI.GetPropertyHeight(steps) + EditorGUI.GetPropertyHeight(description);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty steps = property.FindPropertyRelative(nameof(QuestStepLogic.Step)).FindPropertyRelative(nameof(QuestStepLogic.Step.GUID));
            SerializedProperty description = property.FindPropertyRelative(nameof(QuestStepLogic.Step)).FindPropertyRelative(nameof(QuestStepLogic.Step.Description));

            GUI.enabled = false;

            var height = EditorGUI.GetPropertyHeight(steps);
            Rect rect = new Rect(position.x, position.y, position.width, height);
            EditorGUI.PropertyField(rect, steps);
            position.y += height;

            height = EditorGUI.GetPropertyHeight(description);
            rect = new Rect(position.x, position.y, position.width, height);
            EditorGUI.PropertyField(rect, description);
            position.y += height;

            GUI.enabled = true;

        }

        private void DrawGoals(ref Rect position, SerializedProperty property)
        {
            SerializedProperty goals = property.FindPropertyRelative("m_LogicGoals");

            Rect rect;
            var startPosition = position.y;
            position.x += 10f;
            for (int i = 0; i < goals.arraySize; i++)
            {
                SerializedProperty goalIndex = goals.GetArrayElementAtIndex(i);
                var height = EditorGUI.GetPropertyHeight(goalIndex);
                rect = new Rect(position.x, position.y, position.width - 20f, height);
                EditorGUI.PropertyField(rect, goalIndex);
                position.y += height;
            }
            position.y += EditorGUIUtility.singleLineHeight;

            rect = new Rect(position.x - 3f, startPosition - 3f, position.width - 10, position.y - startPosition);
            Color color = Color.white * 0.1f;
            color.a = 0.25f;
            EditorGUI.DrawRect(rect, color);
        }

        private void DrawLine(ref Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;
            Handles.DrawLine(new Vector2(position.x, position.y), new Vector2(position.width, position.y));
            position.y += EditorGUIUtility.singleLineHeight;
        }
    }
}

