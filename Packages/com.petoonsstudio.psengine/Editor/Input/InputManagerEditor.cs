using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [CustomEditor(typeof(InputManager))]
    public class InputManagerEditor : Editor
    {
        private SerializedProperty m_RebinControllerProperty;
        private void OnEnable()
        {
            InputManager inputManager = target as InputManager;
            m_RebinControllerProperty = serializedObject.FindProperty(nameof(inputManager.RebindController));
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(m_RebinControllerProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("The current Input Manager doesn't support Rebind feature. Add Rebind controller to support Rebinding.",MessageType.Info);
            }
        }
    }
}
