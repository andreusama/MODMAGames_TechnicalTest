using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomPropertyDrawer(typeof(Step))]
    public class StepPropertyDrawer : PropertyDrawer
    {
        private const float LABEL_WIDTH = 0.4f;
        private const float INFO_WIDTH = 0.5f;
        private const float PICKER_BUTTON_WIDTH = 0.1f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty questID = property.FindPropertyRelative("QuestID");
            SerializedProperty stepID = property.FindPropertyRelative("StepID");

            if (questID.propertyType != SerializedPropertyType.String
                || stepID.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, $"[{label.text}] Step attribute only works with string properties.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            DrawLabel(position, label);

            var step = QuestSystemTools.FindStep(stepID.stringValue, out QuestData quest);

            if (step != null)
            {
                questID.stringValue = quest.ID;
                DrawStepInfo(position, $"({questID.stringValue}) S{quest.GetStepReference(step.GUID)}");
            }
            else
            {
                DrawStepInfo(position, $"({questID.stringValue}) [Empty Step Reference]: {stepID.stringValue}");
            }

            DrawPickerButton(position, property, questID, stepID);

            EditorGUI.EndProperty();
        }

        private void DrawLabel(Rect position, GUIContent label)
        {
            Rect textFieldRect = position;
            textFieldRect.width = position.width * LABEL_WIDTH;

            EditorGUI.LabelField(textFieldRect, label);
        }

        private void DrawStepInfo(Rect position, string info)
        {
            Rect textFieldRect = position;
            textFieldRect.x += position.width * LABEL_WIDTH;
            textFieldRect.width = position.width * INFO_WIDTH;

            //EditorGUI.HelpBox(textFieldRect, info, MessageType.None);
            bool guiWasEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.LabelField(textFieldRect, info, GetInfoStyle());
            GUI.enabled = guiWasEnabled;
        }

        private void DrawPickerButton(Rect position, SerializedProperty property, SerializedProperty quest, SerializedProperty step)
        {
            Rect buttonRect = position;
            buttonRect.x += (position.width * LABEL_WIDTH) + (position.width * INFO_WIDTH);
            buttonRect.width = position.width * PICKER_BUTTON_WIDTH;

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_editicon.sml"), EditorStyles.miniButtonRight))
            {
                var data = new StepPickerData(step.stringValue, quest.stringValue);

                var picker = new StepPickerPopupWindowContent(data, property);
                picker.OnClosePickerEvent += Picker_OnClosePickerEvent;

                PopupWindow.Show(buttonRect, picker);
            }
        }

        private void Picker_OnClosePickerEvent(StepPickerData data, SerializedProperty property)
        {
            if (property.serializedObject == null)
                return;

            property.serializedObject.Update();

            property.FindPropertyRelative("QuestID").stringValue = data.QuestID;
            property.FindPropertyRelative("StepID").stringValue = data.StepID;

            property.serializedObject.ApplyModifiedProperties();
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
