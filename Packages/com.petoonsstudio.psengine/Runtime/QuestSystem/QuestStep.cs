using NodeCanvas.BehaviourTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Serializable]
    public class QuestStep
    {
        public enum GoalOperation { All, Any }

        public string GUID = Guid.NewGuid().ToString();
        public GoalOperation GoalCondition;
        public LocalizedString Description;

        public BehaviourTree PreBehaviour;
        public BehaviourTree PostBehaviour;

        [SerializeReference] public List<QuestGoal> Goals;

        public QuestStep()
        {
            Goals = new List<QuestGoal>();
        }

        public int IndexOfGoal(string guid)
        {
            for (int i = 0; i < Goals.Count; i++)
            {
                if (Goals[i].GUID == guid) return i;
            }
            return -1;
        }

        public List<QuestGoal> GetOptionalGoals()
        {
            List<QuestGoal> optionalGoals = Goals.Where((x) => x.IsOptional).ToList();

            if (optionalGoals.Count == Goals.Count)
            {
                // If all are marked as optional, none will be considered optional.
                return new List<QuestGoal>();
            }

            return optionalGoals;
        }
    }
}