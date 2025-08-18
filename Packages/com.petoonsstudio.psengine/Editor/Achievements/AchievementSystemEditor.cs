using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Achievements
{
    [CustomEditor(typeof(AchievementSystem))]
    public class AchievementSystemEditor : Editor
    {
        private AchievementSystem m_Manager;

        private bool m_PreviewFoldout = false;

        private void OnEnable()
        {
            m_Manager = (AchievementSystem)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (m_Manager.AchievementTable != null)
            {
                m_PreviewFoldout = EditorGUILayout.Foldout(m_PreviewFoldout,"Achievements Preview");
                if(m_PreviewFoldout)
                    DrawAchievementPreviews(m_Manager.AchievementTable);
            }
            else
            {
                EditorGUILayout.HelpBox("There's no Achievement Table assigned", MessageType.Info);
            }
        }

        private void DrawAchievementPreviews(AchievementTable table)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Achievements");
            DrawUILine(Color.grey);
            EditorGUI.indentLevel++;
            foreach (var achievementKey in table.Keys)
            {
                EditorGUILayout.LabelField("-" + achievementKey);
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
