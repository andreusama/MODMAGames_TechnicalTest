using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [CustomEditor(typeof(AdditionalContentManager))]
    public class AdditionalContentManagerEditor : Editor
    {
        private AdditionalContentManager m_Manager;

        private void OnEnable()
        {
            m_Manager = (AdditionalContentManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(m_Manager.DownloadableContentTable != null)
            {
                DrawDLCPreviews(m_Manager.DownloadableContentTable);
            }
            else
            {
                EditorGUILayout.HelpBox("There's no Additional Content Table assigned", MessageType.Info);
            }
        }

        private void DrawDLCPreviews(DownloadableContentTable table)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("DLC's");
            DrawUILine(Color.grey);
            EditorGUI.indentLevel++;
            foreach (var dlcKey in table.Keys)
            {
                EditorGUILayout.LabelField("-" + dlcKey);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 2)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
