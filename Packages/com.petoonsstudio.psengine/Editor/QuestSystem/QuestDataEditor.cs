using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CustomEditor(typeof(QuestData), true)]
    public class QuestDataEditor : Editor
    {
        SerializedProperty m_ID;
        SerializedProperty m_Name;
        SerializedProperty m_Description;
        SerializedProperty m_Steps;
        SerializedProperty m_QuestRequirements;
        SerializedProperty m_QuestRewards;
        SerializedProperty m_Repeatable;
        SerializedProperty m_Logic;

        private QuestData m_Quest;

        private int m_AddingNewGoal = -1;

        private List<Type> m_GoalTypes;
        private List<Type> m_RequirementTypes;
        private List<Type> m_RewardTypes;

        private List<bool> m_StepFoldoutVisibilities;

        private bool m_StepsFoldout;
        private bool m_RequirementsFouldout;
        private bool m_RewardsFoldout;

        private bool m_AddingRequirement;
        private bool m_AddingQuestReward;

        protected virtual void OnEnable()
        {
            m_Quest = (QuestData)target;

            m_StepFoldoutVisibilities = new List<bool>();

            foreach (var step in m_Quest.QuestSteps)
            {
                m_StepFoldoutVisibilities.Add(false);
            }

            m_ID = serializedObject.FindProperty("ID");
            m_Name = serializedObject.FindProperty("Name");
            m_Description = serializedObject.FindProperty("Description");
            m_Steps = serializedObject.FindProperty("QuestSteps");
            m_QuestRequirements = serializedObject.FindProperty("QuestRequirements");
            m_QuestRewards = serializedObject.FindProperty("QuestRewards");
            m_Repeatable = serializedObject.FindProperty("Repeatable");
            m_Logic = serializedObject.FindProperty("Logic");

            m_GoalTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(x => x.GetTypes())
                            .Where(x => x.IsClass && x.IsSubclassOf(typeof(QuestGoal))).ToList();

            m_RequirementTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(x => x.GetTypes())
                            .Where(x => x.IsClass && typeof(IQuestRequirements).IsAssignableFrom(x)).ToList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawContent();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawContent()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(m_ID);
            EditorGUILayout.PropertyField(m_Name);
            EditorGUILayout.PropertyField(m_Description);
            EditorGUILayout.PropertyField(m_Repeatable);

            m_RequirementsFouldout = EditorGUILayout.Foldout(m_RequirementsFouldout, "Requirements");
            if (m_RequirementsFouldout)
            {
                DrawRequirementsList();
            }

            m_StepsFoldout = EditorGUILayout.Foldout(m_StepsFoldout, "Steps");
            if (m_StepsFoldout)
            {
                DrawStepList();
            }

            EditorGUILayout.PropertyField(m_Logic);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_Quest);
            }
        }

        private void DrawStepList()
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < m_Quest.QuestSteps.Count; i++)
            {
                DrawStep(i);
            }

            EditorGUI.indentLevel--;

            DrawUILine(Color.gray);

            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));

            if (GUILayout.Button("Add new step"))
            {
                m_Quest.QuestSteps.Add(new QuestStep());
                m_StepFoldoutVisibilities.Add(false);
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);
        }

        private void DrawRequirementsList()
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < m_Quest.QuestRequirements.Count; i++)
            {
                DrawRequirement(i);
            }

            EditorGUI.indentLevel--;

            DrawUILine(Color.gray);

            DrawAddRequirementButton();

            GUILayout.Space(10);
        }

        protected virtual void DrawStep(int i)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"Element {i}");
            EditorGUILayout.PropertyField(m_Steps.GetArrayElementAtIndex(i).FindPropertyRelative("GUID"));
            EditorGUILayout.PropertyField(m_Steps.GetArrayElementAtIndex(i).FindPropertyRelative("Description"));
            EditorGUILayout.PropertyField(m_Steps.GetArrayElementAtIndex(i).FindPropertyRelative("GoalCondition"));
            EditorGUILayout.PropertyField(m_Steps.GetArrayElementAtIndex(i).FindPropertyRelative("PreBehaviour"));
            EditorGUILayout.PropertyField(m_Steps.GetArrayElementAtIndex(i).FindPropertyRelative("PostBehaviour"));

            m_StepFoldoutVisibilities[i] = EditorGUILayout.Foldout(m_StepFoldoutVisibilities[i], "Goals");
            if (m_StepFoldoutVisibilities[i])
            {
                if (m_Quest.QuestSteps[i] != null)
                {
                    GUILayout.BeginVertical();
                    var goals = m_Steps.GetArrayElementAtIndex(i).FindPropertyRelative("Goals");
                    DrawGoals(goals, i);
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();

            DrawReorderButtons(m_Quest.QuestSteps, i, m_Quest.QuestSteps.Count - 1);
            GUILayout.EndHorizontal();
        }

        protected virtual void DrawRequirement(int i)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (m_Quest.QuestRequirements[i] != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(new GUIContent($"Type: {m_Quest.QuestRequirements[i].GetType().Name}"), EditorStyles.centeredGreyMiniLabel);
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(m_QuestRequirements.GetArrayElementAtIndex(i));
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            DrawReorderButtons(m_Quest.QuestRequirements, i, m_Quest.QuestRequirements.Count - 1);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        private void DrawGoals(SerializedProperty goals, int step)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            for (int i = 0; i < goals.arraySize; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                GUILayout.BeginVertical();
                GUILayout.Label(new GUIContent($"Type: {m_Quest.QuestSteps[step].Goals[i].GetType().Name}"), EditorStyles.centeredGreyMiniLabel);

                GUILayout.FlexibleSpace();
                try
                {
                    EditorGUILayout.PropertyField(goals.GetArrayElementAtIndex(i));
                }
                catch
                {
                    Debug.Log("Esto te sale porque los de Unity son unos vagos y no arreglan lo del ReorderableList.");
                }
                GUILayout.FlexibleSpace();

                GUILayout.EndVertical();

                DrawReorderButtons(m_Quest.QuestSteps[step].Goals, i, m_Quest.QuestSteps[step].Goals.Count - 1);

                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            DrawAddGoalsButton(step);
            GUILayout.EndVertical();
        }

        private void DrawReorderButtons<T>(IList<T> list, int position, int lastPosition)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(30));
            GUILayout.FlexibleSpace();

            if (position != 0)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("UpArrow"), GUILayout.Width(30), GUILayout.Height(30)))
                {
                    SwapSteps(list, position, position - 1);
                    if (typeof(T) == typeof(QuestStep))
                    {
                        SwapSteps(m_StepFoldoutVisibilities, position, position - 1);
                    }
                }
            }

            if (position != lastPosition)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_icon dropdown"), GUILayout.Width(30), GUILayout.Height(30)))
                {
                    SwapSteps(list, position, position + 1);
                    if (typeof(T) == typeof(QuestStep))
                    {
                        SwapSteps(m_StepFoldoutVisibilities, position, position + 1);
                    }
                }
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("winbtn_win_close_a"), GUILayout.Width(30), GUILayout.Height(30)))
            {
                list.RemoveAt(position);
                if (typeof(T) == typeof(QuestStep))
                {
                    m_StepFoldoutVisibilities.RemoveAt(position);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void SwapSteps<T>(IList<T> list, int a, int b)
        {
            Swap(list, a, b);
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private void DrawAddGoalsButton(int i)
        {
            if (m_AddingNewGoal == i)
            {
                foreach (var type in m_GoalTypes)
                {
                    if (GUILayout.Button(new GUIContent(type.Name)))
                    {
                        m_Quest.QuestSteps[i].Goals.Add((QuestGoal)Activator.CreateInstance(type));
                        m_AddingNewGoal = -1;
                    }
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus")))
                {
                    m_AddingNewGoal = -1;
                }
            }
            else
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus")))
                {
                    m_AddingNewGoal = i;
                }
            }
        }

        private void DrawAddRequirementButton()
        {
            if (m_AddingRequirement)
            {
                foreach (var type in m_RequirementTypes)
                {
                    if (GUILayout.Button(new GUIContent(type.Name)))
                    {
                        m_Quest.QuestRequirements.Add((IQuestRequirements)Activator.CreateInstance(type));
                        m_AddingRequirement = false;
                    }
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus")))
                {
                    m_AddingRequirement = false;
                }
            }
            else
            {
                if (GUILayout.Button("Add new requirement"))
                {
                    m_AddingRequirement = true;
                }
            }
        }
    }
}

