using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [CustomPropertyDrawer(typeof(AntiAliasingSettingsAttribute))]
    public class AntiAliasingSettingsDrawer : PropertyDrawer
    {
        private static readonly string[] m_AntiAliasingNames = { "x0", "x2", "x4", "x8" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, m_AntiAliasingNames);
        }
    }
}

