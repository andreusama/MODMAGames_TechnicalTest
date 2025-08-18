using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [System.Serializable]
    public class GoalPickerData
    {
        public string GoalID;
        public string StepID;
        public string QuestID;

        public GoalPickerData()
        {
            GoalID = string.Empty;
            StepID = string.Empty;
            QuestID = string.Empty;
        }

        public GoalPickerData(string goalID, string stepID, string questID)
        {
            GoalID = goalID;
            StepID = stepID;
            QuestID = questID;
        }
    }

    public class GoalPickerPopupWindowContent : PopupWindowContent
    {
        private GoalPickerData m_Data;
        private SerializedProperty m_Property;

        private List<QuestData> m_Quests;
        private int m_SelectedQuest = -1;
        private int m_SelectedStep = -1;
        private int m_SelectedGoal = -1;
        private bool m_ShowTextField = false;

        public delegate void OnClosePicker(GoalPickerData data, SerializedProperty property);
        public event OnClosePicker OnClosePickerEvent;

        private const float QUEST_WIDTH = 0.25f;
        private const float STEP_WIDTH = 0.25f;
        private const float GOAL_WIDTH = 0.40f;
        private const float TOGGLE_WIDTH = 0.1f;
        private const float APPLY_WIDTH = 0.2f;
        private const float HEIGHT = 19f;

        private string DataValue { get => m_Data.GoalID; set => m_Data.GoalID = value; }

        public GoalPickerPopupWindowContent(GoalPickerData data, SerializedProperty property)
        {
            m_Data = data;
            m_Property = property;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(600, 60);
        }

        public override void OnGUI(Rect position)
        {
            position.width = editorWindow.position.width;
            position.height = editorWindow.position.height;

            position.x += 10;
            position.y += 10;
            position.width -= 20;
            position.height -= 10;

            if (m_Quests == null) m_Quests = QuestSystemTools.FetchQuestAssets();

            if (m_Quests.Count < 1)
            {
                GUI.Label(position, "No quest assets exist.");
            }

            string currentValue = DataValue;
            bool currentValueIsEmpty = string.IsNullOrEmpty(currentValue);

            if (currentValueIsEmpty)
            {
                if (m_ShowTextField)
                {
                    DrawTextBoxGUI(position);
                }
                else
                {
                    if (m_SelectedQuest < 1) m_SelectedQuest = 0;
                    DrawPopupGUI(position);
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
                    DrawTextBoxGUI(position);
                else
                    DrawPopupGUI(position);
            }

            m_ShowTextField = DrawToggle(position);

            DrawApplyButton(position);
        }

        private void DrawPopupGUI(Rect position)
        {
            m_SelectedQuest = DrawQuestPopup(position, m_SelectedQuest);

            if (DataValue != string.Empty)
            {
                FindGoalInsideQuest(m_SelectedQuest, DataValue, out int foundSelectedStep, out int foundSelectedGoal);
                m_SelectedStep = foundSelectedStep;
                m_SelectedGoal = foundSelectedGoal;
            }

            if (m_SelectedGoal < 0) DataValue = string.Empty;

            m_SelectedStep = DrawStepPopup(position, m_SelectedStep);

            if (m_SelectedQuest >= 0 && m_SelectedStep >= 0)
            {
                m_SelectedGoal = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].IndexOfGoal(DataValue);
                if (m_SelectedGoal < 0) DataValue = string.Empty;
            }

            m_SelectedGoal = DrawGoalPopup(position, m_SelectedGoal);

            if (m_SelectedQuest >= 0 && m_SelectedStep >= 0 && m_SelectedGoal >= 0)
            {
                m_Data.QuestID = m_Quests[m_SelectedQuest].ID;
                m_Data.StepID = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].GUID;
                m_Data.GoalID = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].Goals[m_SelectedGoal].GUID;
            }  
        }

        private void DrawTextBoxGUI(Rect position)
        {
            m_ShowTextField = true;
            DataValue = DrawTextField(position, DataValue);
        }

        private int DrawQuestPopup(Rect parentRect, int currentIndex)
        {
            Rect popupRect = parentRect;
            popupRect.position = new Vector2(popupRect.position.x, popupRect.position.y);
            popupRect.width = parentRect.width * QUEST_WIDTH;
            popupRect.height = HEIGHT;

            return EditorGUI.Popup(popupRect, currentIndex, m_Quests.Select((quest) => quest.ID).ToArray());
        }

        private int DrawStepPopup(Rect parentRect, int currentIndex)
        {
            Rect popupRect = parentRect;
            popupRect.position = new Vector2(popupRect.position.x + (parentRect.width * QUEST_WIDTH), popupRect.position.y);
            popupRect.width = parentRect.width * STEP_WIDTH;
            popupRect.height = HEIGHT;

            var steps = m_Quests[m_SelectedQuest].QuestSteps;

            string[] options = new string[steps.Count];

            for (int i = 0; i < steps.Count; i++)
            {
                options[i] = $"{i}: {QuestSystemTools.GenerateStepDescription(steps[i])}";
            }

            return EditorGUI.Popup(popupRect, currentIndex, options.ToArray());
        }

        private int DrawGoalPopup(Rect parentRect, int currentIndex)
        {
            Rect popupRect = parentRect;
            popupRect.position = new Vector2(popupRect.position.x + (parentRect.width * QUEST_WIDTH) + (parentRect.width * STEP_WIDTH), popupRect.position.y);
            popupRect.width = parentRect.width * GOAL_WIDTH;
            popupRect.height = HEIGHT;

            if (m_SelectedStep < 0)
            {
                return EditorGUI.Popup(popupRect, currentIndex, new string[] { });
            }

            var goals = m_Quests[m_SelectedQuest].QuestSteps[m_SelectedStep].Goals;

            string[] options = new string[goals.Count];

            for (int i = 0; i < goals.Count; i++)
            {
                options[i] = $"{i}: {QuestSystemTools.GenerateGoalDescription(goals[i])}";
            }

            return EditorGUI.Popup(popupRect, currentIndex, options.ToArray());
        }

        private string DrawTextField(Rect parentRect, string currentValue)
        {
            Rect textFieldRect = parentRect;
            textFieldRect.width = (parentRect.width * QUEST_WIDTH) + (parentRect.width * STEP_WIDTH) + (parentRect.width * GOAL_WIDTH);
            textFieldRect.height = HEIGHT;

            return EditorGUI.TextField(textFieldRect, currentValue);
        }

        private bool DrawToggle(Rect parentRect)
        {
            float offset = (parentRect.width * QUEST_WIDTH) + (parentRect.width * STEP_WIDTH) + (parentRect.width * GOAL_WIDTH);

            Rect toggleRect = parentRect;
            toggleRect.position = new Vector2(toggleRect.position.x + offset, toggleRect.position.y);
            toggleRect.width = parentRect.width * TOGGLE_WIDTH;
            toggleRect.height = HEIGHT;

            if (GUI.Button(toggleRect, EditorGUIUtility.IconContent(m_ShowTextField ? "d_ScriptableObject Icon" : "d_InputField Icon")))
            {
                m_ShowTextField = !m_ShowTextField;
            }

            return m_ShowTextField;
        }

        private void DrawApplyButton(Rect parentRect)
        {
            var buttonWidth = parentRect.width * APPLY_WIDTH;

            Rect applyButtonRect = parentRect;
            applyButtonRect.y += HEIGHT;
            applyButtonRect.height = HEIGHT;

            var buttonContent = new GUIContent(EditorGUIUtility.IconContent("Installed"))
            {
                text = " Apply  "
            };

            if (GUI.Button(applyButtonRect, buttonContent, EditorStyles.miniButton))
            {
                editorWindow.Close();
            }
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

        public override void OnClose()
        {
            OnClosePickerEvent?.Invoke(m_Data, m_Property);
        }
    }
}
