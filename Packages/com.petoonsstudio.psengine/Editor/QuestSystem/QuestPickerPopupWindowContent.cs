using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [System.Serializable]
    public class QuestPickerData
    {
        public string QuestID;
        public SerializedProperty Property;

        public QuestPickerData(SerializedProperty property)
        {
            QuestID = property.stringValue;
            Property = property;
        }

        public void ApplyData()
        {
            try
            {
                Property.stringValue = QuestID;
            }
            catch 
            { 
                Debug.LogError("Tried to apply data to inexistant property or lost serializedObject."); 
            }
        }
    }

    public class QuestPickerPopupWindowContent : PopupWindowContent
    {
        private QuestPickerData m_Data;

        private List<string> m_Quests;
        private bool m_ShowTextField = false;

        public Action<QuestPickerData> OnClosePicker;

        private const float DATA_WIDTH = 0.8f;
        private const float TOGGLE_WIDTH = 0.2f;
        private const float APPLY_WIDTH = 0.2f;
        private const float HEIGHT = 19f;

        private string DataValue { get => m_Data.QuestID; set => m_Data.QuestID = value; }

        public QuestPickerPopupWindowContent(QuestPickerData data)
        {
            m_Data = data;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 60);
        }

        public override void OnGUI(Rect position)
        {
            position.width = editorWindow.position.width;
            position.height = editorWindow.position.height;

            position.x += 10;
            position.y += 10;
            position.width -= 20;
            position.height -= 10;

            if (m_Quests == null) m_Quests = QuestSystemTools.FetchQuestAssets().Select((x) => x.ID).ToList();

            if (m_Quests.Count < 1)
            {
                GUI.Label(position, "No quest assets exist.");
            }

            string currentValue = DataValue;

            int selectedValue = m_Quests.IndexOf(currentValue);
            if (selectedValue < 0 && !string.IsNullOrEmpty(currentValue)) m_ShowTextField = true;

            if (m_ShowTextField)
            {
                DataValue = DrawTextField(position, currentValue);
            }
            else
            {
                int popupValue = DrawPopup(position, selectedValue);

                if (popupValue >= 0)
                    DataValue = m_Quests[popupValue];
            }

            m_ShowTextField = DrawToggle(position);

            DrawApplyButton(position);
        }

        private int DrawPopup(Rect parentRect, int currentIndex)
        {
            Rect popupRect = parentRect;
            popupRect.width = parentRect.width * DATA_WIDTH;
            popupRect.height = HEIGHT;

            return EditorGUI.Popup(popupRect, currentIndex, m_Quests.ToArray());
        }

        private string DrawTextField(Rect parentRect, string currentValue)
        {
            m_ShowTextField = true;

            Rect textFieldRect = parentRect;
            textFieldRect.width = parentRect.width * DATA_WIDTH;
            textFieldRect.height = HEIGHT;

            return EditorGUI.TextField(textFieldRect, currentValue);
        }

        private bool DrawToggle(Rect parentRect)
        {
            float offset = (parentRect.width * DATA_WIDTH);

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

        public override void OnClose()
        {
            OnClosePicker?.Invoke(m_Data);
        }
    }
}
