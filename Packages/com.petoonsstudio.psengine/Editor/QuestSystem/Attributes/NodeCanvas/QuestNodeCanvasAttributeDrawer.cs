using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class QuestNodeCanvasAttributeDrawer : AttributeDrawer<QuestNodeCanvasAttribute>
    {
        private List<QuestData> m_Quests;
        
        private bool m_ShowTextField = false;

        public override object OnGUI(GUIContent content, object instance)
        {
            if (fieldInfo.FieldType != typeof(string))
            {
                GUILayout.Label($"[{content.text}] Step attribute only works with StepNodeData properties.");
                return instance;
            }

            if (m_Quests == null) m_Quests = QuestSystemTools.FetchQuestAssets();

            if (m_Quests.Count < 1)
            {
                GUILayout.Label("No quest assets exist.");
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(content);

            string currentValue;

            if (instance == null)
                currentValue = string.Empty;
            else
                currentValue = instance as string;

            if (m_ShowTextField)
                DrawTextBoxGUI(ref currentValue);
            else
                DrawPopupGUI(ref currentValue);

            m_ShowTextField = DrawToggle();

            GUILayout.EndHorizontal();

            return currentValue;
        }

        private int GetCurrentQuestIndex(string questID)
        {
            for (int i = 0; i < m_Quests.Count; i++)
            {
                if (m_Quests[i].ID == questID)
                    return i;
            }

            return -1;
        }

        private void DrawPopupGUI(ref string currentValue)
        {
            int questIndex = GetCurrentQuestIndex(currentValue);
            questIndex = DrawQuestPopup(questIndex);

            if (questIndex >= 0)
            {
                currentValue = m_Quests[questIndex].ID;
            }
            else
            {
                currentValue = string.Empty;
            }
        }

        private int DrawQuestPopup(int currentIndex)
        {
            return EditorGUILayout.Popup(currentIndex, m_Quests.Select((quest) => quest.ID).ToArray(), GUILayout.MaxWidth(80));
        }

        private void DrawTextBoxGUI(ref string currentValue)
        {
            m_ShowTextField = true;
            currentValue = EditorGUILayout.TextField(currentValue);
        }

        private bool DrawToggle()
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent(m_ShowTextField ? "d_ScriptableObject Icon" : "d_InputField Icon"), GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                m_ShowTextField = !m_ShowTextField;
            }

            return m_ShowTextField;
        }
    }
}
