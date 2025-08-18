using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [CustomPropertyDrawer(typeof(DataSettings), true)]
    public class DataSettingsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = property.FindPropertyRelative("Type");
            var height = EditorGUI.GetPropertyHeight(type);
            position.height = height;
            EditorGUI.PropertyField(position, type);
            position.y += height + 2;
        }
    }
}

