using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class QuestActiveRequirement : IQuestRequirements
    {
        public string QuestID;
        public bool InvertCondition;

        public bool RequirementCompleted()
        {
            if (!InvertCondition)
            {
                return QuestSystem.Instance.IsQuestActive(QuestID);
            }
            else
            {
                return !QuestSystem.Instance.IsQuestActive(QuestID);
            }
        }
    }
}

