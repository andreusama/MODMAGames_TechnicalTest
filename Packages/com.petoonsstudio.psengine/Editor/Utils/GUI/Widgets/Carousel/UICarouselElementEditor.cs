using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomEditor(typeof(UICarouselElement)), CanEditMultipleObjects]
    public class UICarouselElementEditor : ButtonEditor
    {
        private SerializedProperty m_SelectAnim;
        private SerializedProperty m_DeselectAnim;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_SelectAnim = serializedObject.FindProperty("SelectAnim");
            m_DeselectAnim = serializedObject.FindProperty("DeselectAnim");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle header = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };


            GUILayout.Space(5);
            GUILayout.Label("Button Behaviour", header);
            base.OnInspectorGUI();

            GUILayout.Label("Animations", header);
            EditorGUILayout.PropertyField(m_SelectAnim, new GUIContent("Select Animation"));
            EditorGUILayout.PropertyField(m_DeselectAnim, new GUIContent("Deselect Animation"));

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnDisable()
        {
            UICarouselElement element = (UICarouselElement)target;

            if (element)
                Undo.RecordObject(element, "UICarouselElement changed");

            base.OnDisable();
        }
    } 
}
