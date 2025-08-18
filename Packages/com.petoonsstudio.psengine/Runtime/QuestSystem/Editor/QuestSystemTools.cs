using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class QuestSystemTools
    {
        private static Dictionary<string, QuestData> m_CachedQuestAssets;

        public static QuestData FindQuest(string id)
        {
            if (m_CachedQuestAssets == null) FetchCachedQuestAssets();
            if (m_CachedQuestAssets.TryGetValue(id, out QuestData quest))
            {
                return quest;
            }
            return null;
        }

        public static QuestStep FindStep(string id, out QuestData quest)
        {
            if (m_CachedQuestAssets == null) FetchCachedQuestAssets();
            foreach (var asset in m_CachedQuestAssets)
            {
                quest = asset.Value;
                var step = quest.GetStep(id);
                if (step != null) return step;
            }

            quest = null;
            return null;
        }

        public static QuestGoal FindGoal(string id, out QuestData quest, out QuestStep step)
        {
            if (m_CachedQuestAssets == null) FetchCachedQuestAssets();
            foreach (var asset in m_CachedQuestAssets)
            {
                foreach (var questStep in asset.Value.QuestSteps)
                {
                    foreach (var questGoal in questStep.Goals)
                    {
                        if (questGoal.GUID == id)
                        {
                            quest = asset.Value;
                            step = questStep;
                            return questGoal;
                        }
                    }
                }
            }

            quest = null;
            step = null;
            return null;
        }

        public static QuestGoal FindGoal(string questID, string stepID, string goalID)
        {
            if (m_CachedQuestAssets == null) FetchCachedQuestAssets();

            if (m_CachedQuestAssets.TryGetValue(questID, out QuestData quest))
            {
                foreach (var questStep in quest.QuestSteps)
                {
                    if (questStep.GUID != stepID)
                        continue;

                    foreach (var questGoal in questStep.Goals)
                    {
                        if (questGoal.GUID == goalID)
                        {
                            return questGoal;
                        }
                    }
                }
            }

            return null;
        }

        public static List<QuestData> FetchQuestAssets(bool sortAlphabetically = true)
        {
            var questAssetGUIDs = AssetDatabase.FindAssets($"t:{nameof(QuestData)}");

            var questAssets = new List<QuestData>(questAssetGUIDs.Length);

            foreach (string guid in questAssetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                QuestData questAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject)) as QuestData;

                questAssets.Add(questAsset);
            }

            if (sortAlphabetically)
            {
                questAssets = questAssets.OrderBy((x) => x.ID).ToList();
            }

            return questAssets;
        }

        public static string GenerateStepDescription(QuestStep step, int maxStepLength = 0, int maxGoalLength = 0, bool includeCounter = true)
        {
            if (step == null || step.Goals.Count < 1) return string.Empty;

            string description;

            if (step.Goals.Count == 1)
            {
                description = GenerateGoalDescription(step.Goals[0], maxGoalLength);

                if (maxStepLength > 0 && maxStepLength < description.Length)
                {
                    description = description.Substring(0, maxStepLength);
                    description += "…";
                }

                return description;
            }

            description = includeCounter ? $"({step.Goals.Count}) " : "";
            int baseLength = description.Length;
            bool isFirstGoal = true;

            foreach (var goal in step.Goals)
            {
                if (!isFirstGoal) description += ", ";
                description += GenerateGoalDescription(goal);
                isFirstGoal = false;
            }

            if (maxStepLength > 0 && baseLength + maxStepLength < description.Length)
            {
                description = description.Substring(0, baseLength + maxStepLength);
                description += "…";
            }

            return description;
        }

        public static string GenerateGoalDescription(QuestGoal goal, int maxLength = 0)
        {
            string description;

            if (goal is IQuestGoalInfo info) description = info.Info;
            else description = goal.GUID;

            if (maxLength > 0 && maxLength < description.Length)
            {
                description = description.Substring(0, maxLength);
                description += "…";
            }

            return description;
        }

        private static void FetchCachedQuestAssets()
        {
            var quests = FetchQuestAssets();

            m_CachedQuestAssets = new Dictionary<string, QuestData>();

            foreach (var quest in quests)
            {
                m_CachedQuestAssets.Add(quest.ID, quest);
            }
        }
    }
}

#endif