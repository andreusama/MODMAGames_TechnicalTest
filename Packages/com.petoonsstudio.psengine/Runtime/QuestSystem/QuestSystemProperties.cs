using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Serializable]
    public struct Goal
    {
        public string GoalID;
        public string StepID;
        public string QuestID;

        public Goal(string goalID, string stepID, string questID)
        {
            GoalID = goalID;
            StepID = stepID;
            QuestID = questID;
        }
    }

    [Serializable]
    public struct Step
    {
        public string QuestID;
        public string StepID;

        public Step(string questID, string stepID)
        {
            QuestID = questID;
            StepID = stepID;
        }
    }
}