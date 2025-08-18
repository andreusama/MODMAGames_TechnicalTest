using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class QuestCompletedRequirement : IQuestRequirements
    {
        public string QuestID;
        public bool InvertCondition;

        public bool RequirementCompleted()
        {
            if (!InvertCondition)
            {
                return QuestSystem.Instance.IsQuestCompleted(QuestID);
            }
            else
            {
                return !QuestSystem.Instance.IsQuestCompleted(QuestID);
            }
        }
    }
}

