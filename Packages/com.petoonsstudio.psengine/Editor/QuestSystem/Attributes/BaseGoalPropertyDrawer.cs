using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public abstract class BaseGoalPropertyDrawer : PropertyDrawer
    {
        private float m_AdditionalHeight;

        public virtual List<SerializedProperty> SetProperties(SerializedProperty goalProperty)
        {
            var guid = goalProperty.FindPropertyRelative(nameof(QuestGoal.GUID));
            var description = goalProperty.FindPropertyRelative(nameof(QuestGoal.Description));
            var isOptional = goalProperty.FindPropertyRelative(nameof(QuestGoal.IsOptional));
            var postActions = goalProperty.FindPropertyRelative(nameof(QuestGoal.PostActions));

            var properties = new List<SerializedProperty>();

            if (guid != null) properties.Add(guid);
            if (description != null) properties.Add(description);
            if (isOptional != null) properties.Add(isOptional);
            if (postActions != null) properties.Add(postActions);

            return properties;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var properties = SetProperties(property);

            EditorGUI.indentLevel++;

            m_AdditionalHeight = PrePropertiesGUI(ref position, property, properties);

            foreach (var relativeProperty in properties)
            {
                DrawProperty(ref position, relativeProperty);
            }

            m_AdditionalHeight += PostPropertiesGUI(ref position, property, properties);

            EditorGUI.indentLevel--;
        }

        public virtual float PrePropertiesGUI(ref Rect initialPosition, SerializedProperty goal, List<SerializedProperty> properties) { return 0f; }

        public virtual float PostPropertiesGUI(ref Rect finalPosition, SerializedProperty goal, List<SerializedProperty> properties) { return 0f; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var properties = SetProperties(property);

            float height = 0f;

            for (int i = 0; i < properties.Count; i++)
            {
                height += EditorGUI.GetPropertyHeight(properties[i], true) + 6f;
            }

            height += m_AdditionalHeight;

            return height - 16f;
        }

        protected void DrawProperty(ref Rect position, SerializedProperty property)
        {
            if (property == null) return;

            float height = EditorGUI.GetPropertyHeight(property);

            Rect childPropertyRect = position;
            childPropertyRect.height = height;

            EditorGUI.PropertyField(childPropertyRect, property, true);
            position.y += height + 2;
        }
    }
}