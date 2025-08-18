using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    [CustomEditor(typeof(SliderWithValue))]
    public class SliderWithValueEditor : SliderEditor
    {
        private SerializedProperty m_ValueText;
        private SerializedProperty m_Multiplier;
        private SerializedProperty m_Format;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_ValueText = serializedObject.FindProperty("ValueText");
            m_Multiplier = serializedObject.FindProperty("Multiplier");
            m_Format = serializedObject.FindProperty("Format");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_ValueText);
            EditorGUILayout.PropertyField(m_Multiplier);
            EditorGUILayout.PropertyField(m_Format);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
