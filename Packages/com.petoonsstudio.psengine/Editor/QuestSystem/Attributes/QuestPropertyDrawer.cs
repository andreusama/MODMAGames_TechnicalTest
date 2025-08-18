using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomPropertyDrawer(typeof(QuestAttribute))]
    public class QuestPropertyDrawer : PropertyDrawer
    {
        private static List<QuestPickerData> m_PickerDatasToBeApplied;

        private const float LABEL_WIDTH = 0.4f;
        private const float INFO_WIDTH = 0.5f;
        private const float PICKER_BUTTON_WIDTH = 0.1f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                GUI.Label(position, $"[{label.text}] Quest attribute only works with string properties.");
                return;
            }

            if (m_PickerDatasToBeApplied == null) m_PickerDatasToBeApplied = new List<QuestPickerData>();

            EditorGUI.BeginProperty(position, label, property);

            foreach (var dataToApply in m_PickerDatasToBeApplied)
            {
                dataToApply.ApplyData();
            }

            m_PickerDatasToBeApplied.Clear();

            DrawLabel(position, label);

            var quest = QuestSystemTools.FindQuest(property.stringValue);

            if (quest != null)
            {
                DrawQuestInfo(position, quest.ID);
            }
            else
            {
                DrawQuestInfo(position, string.IsNullOrEmpty(property.stringValue) ? "[Empty Quest Reference]" : $"(!) {property.stringValue}");
            }

            //DrawTextField(position, property, label);
            DrawPickerButton(position, property);

            EditorGUI.EndProperty();
        }

        private void DrawLabel(Rect parentRect, GUIContent label)
        {
            Rect labelRect = parentRect;
            labelRect.width = parentRect.width * LABEL_WIDTH;

            GUI.Label(labelRect, label);
        }

        private void DrawQuestInfo(Rect position, string info)
        {
            Rect textFieldRect = position;
            textFieldRect.x += position.width * LABEL_WIDTH;
            textFieldRect.width = position.width * INFO_WIDTH;

            //EditorGUI.HelpBox(textFieldRect, info, MessageType.None);
            bool guiWasEnabled = GUI.enabled;
            GUI.enabled = false;
            GUI.Label(textFieldRect, info, GetInfoStyle());
            GUI.enabled = guiWasEnabled;
        }

        private void DrawPickerButton(Rect position, SerializedProperty property)
        {
            Rect buttonRect = position;
            buttonRect.x += (position.width * LABEL_WIDTH) + (position.width * INFO_WIDTH);
            buttonRect.width = position.width * PICKER_BUTTON_WIDTH;

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_editicon.sml"), EditorStyles.miniButtonRight))
            {
                var data = new QuestPickerData(property);

                var picker = new QuestPickerPopupWindowContent(data);

                picker.OnClosePicker += OnClosePicker;

                PopupWindow.Show(buttonRect, picker);
            }
        }

        private static void OnClosePicker(QuestPickerData data)
        {
            m_PickerDatasToBeApplied.Add(data);
        }

        private GUIStyle GetInfoStyle()
        {
            return new GUIStyle(EditorStyles.miniButtonLeft)
            {
                alignment = TextAnchor.MiddleLeft
            };
        }
    }
}
