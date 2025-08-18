using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomPropertyDrawer(typeof(QuestGoalLogic), true)]
    public class QuestGoalLogicPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty goal = property.FindPropertyRelative("Goal");
            SerializedProperty completed = property.FindPropertyRelative("IsCompleted");

            return EditorGUI.GetPropertyHeight(goal) + EditorGUI.GetPropertyHeight(completed);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty goal = property.FindPropertyRelative("Goal");
            SerializedProperty completed = property.FindPropertyRelative("IsCompleted");

            GUI.enabled = false;
            var goalHeight = EditorGUI.GetPropertyHeight(goal);
            Rect goalRect = new Rect(position.x, position.y, position.width, goalHeight);
            EditorGUI.PropertyField(goalRect, goal);
            position.y += goalHeight;

            Rect completedRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(completed));
            EditorGUI.PropertyField(completedRect, completed);
            GUI.enabled = true;
        }
    }
}

