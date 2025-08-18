using NodeCanvas.BehaviourTrees;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Serializable]
    public abstract class QuestGoal
    {
        public string GUID = Guid.NewGuid().ToString();
        public LocalizedString Description;
        public bool IsOptional;
        public BehaviourTree PostActions;

        public abstract QuestGoalLogic GenerateLogic(QuestStepLogic step);
    }
}