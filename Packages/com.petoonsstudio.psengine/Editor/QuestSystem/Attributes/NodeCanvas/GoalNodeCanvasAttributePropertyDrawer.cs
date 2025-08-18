using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomPropertyDrawer(typeof(GoalNodeCanvasAttribute))]
    public class GoalNodeCanvasAttributePropertyDrawer : AttributeDrawer<GoalNodeCanvasAttribute>
    {
        private List<QuestData> m_Quests;
        private int m_SelectedQuest = -1;
        private int m_SelectedStep = -1;
        private int m_SelectedGoal = -1;
        private bool m_ShowTextField = false;

        public override object OnGUI(GUIContent content, object instance)
        {
            if (fieldInfo.FieldType != typeof(string) && fieldInfo.FieldType != typeof(BBParameter<string>))
            {
                GUILayout.Label($"[{content.text}] Goal attribute only works with string properties.");
                return instance;
            }

            if (m_Quests == null) m_Quests = QuestSystemTools.FetchQuestAssets();

            if (m_Quests.Count < 1)
            {
                GUILayout.Label("No quest assets exist.");
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(content);

            string currentValue = string.Empty;

            if (instance != null)
            {
                if (fieldInfo.FieldType == typeof(string))
                    currentValue = instance as string;
                else if (fieldInfo.FieldType == typeof(BBParameter<string>))
                    currentValue = (instance as BBParameter<string>).value;
            }

            bool currentValueIsEmpty = string.IsNullOrEmpty(currentValue);

            if (currentValueIsEmpty)
            {
                if (m_ShowTextField)
                {
                    currentValue = DrawTextBoxGUI(currentValue);
                }
                else
                {
                    if (m_SelectedQuest < 1) m_SelectedQuest = 0;
                    currentValue = DrawPopupGUI(currentValue);
                }
            }
            else
            {
                if (m_SelectedQuest < 0)
                    m_SelectedQuest = FindQuestForGoal(currentValue, out m_SelectedStep, out m_SelectedGoal);

                if (m_SelectedQuest >= 0 && m_SelectedStep < 0)
                {
                    var index = m_Quests[m_SelectedQuest].GetStepReference(currentValue);
                    m_SelectedStep = index >= 0 ? index : 0;
                }

                if (m_SelectedQuest >= 0 && m_SelectedStep >= 0 && m_SelectedGoal < 0)
                    m_SelectedGoal = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].IndexOfGoal(currentValue);

                if (m_SelectedQuest < 0 || m_SelectedStep < 0 || m_SelectedGoal < 0 || m_ShowTextField)
                    currentValue = DrawTextBoxGUI(currentValue);
                else
                    currentValue = DrawPopupGUI(currentValue);
            }

            m_ShowTextField = DrawToggle();

            GUILayout.EndHorizontal();

            if (fieldInfo.FieldType == typeof(string))
            {
                return currentValue;
            }
            else if (fieldInfo.FieldType == typeof(BBParameter<string>))
            {
                var bbparam = instance as BBParameter<string>;
                bbparam.value = currentValue;
                return bbparam;
            }

            return instance;
        }

        private string DrawPopupGUI(string currentValue)
        {
            m_SelectedQuest = DrawQuestPopup(m_SelectedQuest);

            FindGoalInsideQuest(m_SelectedQuest, currentValue, out int foundSelectedStep, out int foundSelectedGoal);
            if (foundSelectedStep >= 0) m_SelectedStep = foundSelectedStep;
            if (foundSelectedGoal >= 0) m_SelectedGoal = foundSelectedGoal;

            m_SelectedStep = DrawStepPopup(m_SelectedStep);

            if (m_SelectedQuest >= 0 && m_SelectedStep >= 0)
            {
                m_SelectedGoal = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].IndexOfGoal(currentValue);
                if (m_SelectedGoal < 0) currentValue = string.Empty;
            }

            m_SelectedGoal = DrawGoalPopup(m_SelectedGoal);

            if (m_SelectedQuest >= 0 && m_SelectedStep >= 0 && m_SelectedGoal >= 0)
                currentValue = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].Goals[m_SelectedGoal].GUID;

            return currentValue;
        }
        
        private string DrawTextBoxGUI(string currentValue)
        {
            m_ShowTextField = true;
            return EditorGUILayout.TextField(currentValue);
        }

        private int DrawQuestPopup(int currentIndex)
        {
            return EditorGUILayout.Popup(currentIndex, m_Quests.Select((quest) => quest.ID).ToArray());
        }

        private int DrawStepPopup(int currentIndex)
        {
            var steps = m_Quests[m_SelectedQuest].QuestSteps;

            string[] options = new string[steps.Count];

            for (int i = 0; i < steps.Count; i++)
            {
                options[i] = $"{i}: {QuestSystemTools.GenerateStepDescription(steps[i])}";
            }

            return EditorGUILayout.Popup(currentIndex, options.ToArray());
        }

        private int DrawGoalPopup(int currentIndex)
        {
            if (m_SelectedStep < 0)
            {
                return EditorGUILayout.Popup(currentIndex, new string[]{ });
            }

            var goals = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].Goals;

            string[] options = new string[goals.Count];

            for (int i = 0; i < goals.Count; i++)
            {
                options[i] = $"{i}: {QuestSystemTools.GenerateGoalDescription(goals[i])}";
            }

            return EditorGUILayout.Popup(currentIndex, options.ToArray());
        }

        private bool DrawToggle()
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent(m_ShowTextField ? "d_ScriptableObject Icon" : "d_InputField Icon"), GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                m_ShowTextField = !m_ShowTextField;
            }

            return m_ShowTextField;
        }

        private int FindQuestForGoal(string goal, out int stepIndex, out int goalIndex)
        {
            for (int i = 0; i < m_Quests.Count; i++)
            {
                if (FindGoalInsideQuest(i, goal, out stepIndex, out goalIndex))
                    return i;
            }
            stepIndex = -1;
            goalIndex = -1;
            return -1;
        }

        private bool FindGoalInsideQuest(int questIndex, string goalID, out int stepIndex, out int goalIndex)
        {
            for (int i = 0; i < m_Quests[questIndex].QuestSteps.Count; i++)
            {
                goalIndex = m_Quests[questIndex].QuestSteps[i].IndexOfGoal(goalID);
                if (goalIndex >= 0)
                {
                    stepIndex = i;
                    return true;
                }
            }
            stepIndex = -1;
            goalIndex = -1;
            return false;
        }
    }
}
