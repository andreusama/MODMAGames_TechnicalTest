using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class StepNodeCanvasAttributeDrawer : AttributeDrawer<StepNodeCanvasAttribute>
    {
        private List<QuestData> m_Quests;

        private bool m_ShowTextField = false;

        public override object OnGUI(GUIContent content, object instance)
        {
            if (fieldInfo.FieldType != typeof(StepNodeData))
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

            StepNodeData currentValue;

            if (instance == null)
                currentValue = new StepNodeData();
            else
                currentValue = instance as StepNodeData;

            if (string.IsNullOrEmpty(currentValue.Quest))
            {
                currentValue.Step = string.Empty;
            }

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
            for(int i = 0; i < m_Quests.Count; i++)
            {
                if (m_Quests[i].ID == questID)
                    return i;
            }

            return -1;
        }

        private int GetCurrentStepIndex(int questIndex, string stepID)
        {
            for (int i = 0; i <  m_Quests[questIndex].QuestSteps.Count; i++)
            {
                if (m_Quests[questIndex].QuestSteps[i].GUID == stepID)
                {
                    return i;
                }
            }

            return -1;
        }

        private void DrawPopupGUI(ref StepNodeData currentValue)
        {
            int questIndex = GetCurrentQuestIndex(currentValue.Quest);
            questIndex = DrawQuestPopup(questIndex);

            if (questIndex >= 0)
            {
                currentValue.Quest = m_Quests[questIndex].ID;

                int stepIndex = GetCurrentStepIndex(questIndex, currentValue.Step);
                stepIndex = DrawStepPopup(questIndex, stepIndex);

                if (stepIndex >= 0) 
                    currentValue.Step = m_Quests[questIndex].QuestSteps[stepIndex].GUID;
                else
                    currentValue.Step = string.Empty;
            }
            else
            {
                currentValue.Quest = string.Empty;
                currentValue.Step = string.Empty;
            }
        }

        private int DrawQuestPopup(int currentIndex)
        {
            return EditorGUILayout.Popup(currentIndex, m_Quests.Select((quest) => quest.ID).ToArray(), GUILayout.MaxWidth(80));
        }

        private void DrawTextBoxGUI(ref StepNodeData currentValue)
        {
            m_ShowTextField = true;
            currentValue.Quest = EditorGUILayout.TextField(currentValue.Quest);
            currentValue.Step = EditorGUILayout.TextField(currentValue.Step);
        }

        private bool DrawToggle()
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent(m_ShowTextField ? "d_ScriptableObject Icon" : "d_InputField Icon"), GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                m_ShowTextField = !m_ShowTextField;
            }

            return m_ShowTextField;
        }

        private int DrawStepPopup(int questIndex, int currentIndex)
        {
            var steps = m_Quests[questIndex].QuestSteps;

            string[] options = new string[steps.Count];

            for (int i = 0; i < steps.Count; i++)
            {
                options[i] = $"{i}: {QuestSystemTools.GenerateStepDescription(steps[i])}";
            }

            return EditorGUILayout.Popup(currentIndex, options.ToArray());
        }

        private int FindQuestForStep(string step)
        {
            for (int i = 0; i < m_Quests.Count; i++)
            {
                if (m_Quests[i].GetStepReference(step) >= 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
