using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    [CustomEditor(typeof(EnGUIManager), true)]
    public class EnGUIManagerEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawContents();
            DrawRequests();
        }

        private void DrawContents()
        {
            EnGUIManager myComponent = (EnGUIManager)target;
            FieldInfo privateField = typeof(EnGUIManager).GetField("m_Content", BindingFlags.NonPublic | BindingFlags.Instance);

            EditorGUILayout.BeginVertical();

            if (privateField != null)
            {
                Stack<EnGUIContent> privateQueueValue = (Stack<EnGUIContent>)privateField.GetValue(myComponent);

                if (privateQueueValue != null)
                {
                    // Muestra los elementos de la Queue en el Inspector
                    EditorGUILayout.LabelField("EnGUIManager Contents");
                    EditorGUI.indentLevel++;

                    foreach (EnGUIContent item in privateQueueValue)
                        EditorGUILayout.LabelField(item.name);

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Field not found.", MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawRequests()
        {
            EnGUIManager myComponent = (EnGUIManager)target;
            FieldInfo privateField = typeof(EnGUIManager).GetField("m_Requests", BindingFlags.NonPublic | BindingFlags.Instance);

            EditorGUILayout.BeginVertical();

            if (privateField != null)
            {
                Queue<EnGUIManager.IRequest> privateQueueValue = (Queue<EnGUIManager.IRequest>)privateField.GetValue(myComponent);

                if (privateQueueValue != null)
                {
                    // Muestra los elementos de la Queue en el Inspector
                    EditorGUILayout.LabelField("EnGUIManager Requests");
                    EditorGUI.indentLevel++;

                    foreach (EnGUIManager.IRequest item in privateQueueValue)
                        EditorGUILayout.LabelField(item.GetType().ToString());

                    EditorGUI.indentLevel--;
                }
            }
            else
                EditorGUILayout.HelpBox("Field not found.", MessageType.Error);

            EditorGUILayout.EndVertical();
        }
    }
}

