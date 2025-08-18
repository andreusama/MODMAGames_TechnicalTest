using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [CustomPropertyDrawer(typeof(VsyncSettingsAttribute))]
    public class VsyncAttributeDrawer : PropertyDrawer
    {
        private static readonly string[] m_VsyncNames = { "Unlimited", "60fps", "30fps" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, m_VsyncNames);
        }
    }
}
