using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [CustomPropertyDrawer(typeof(QualitySettingsAttribute))]
    public class QualitySettingsAttibuteDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, QualitySettings.names);
        }
    }
}