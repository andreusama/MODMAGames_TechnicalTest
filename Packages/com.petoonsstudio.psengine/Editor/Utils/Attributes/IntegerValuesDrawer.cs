using UnityEngine;
using UnityEditor;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomPropertyDrawer(typeof(IntegerValuesAttribute))]
    public class IntegerValuesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, (attribute as IntegerValuesAttribute).Values);
        }
    }
}

