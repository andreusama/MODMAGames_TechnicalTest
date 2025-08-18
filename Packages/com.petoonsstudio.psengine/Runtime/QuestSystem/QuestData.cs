using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Petoons Studio/PSEngine/Quest System/Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Data")]
        public string ID;
        public LocalizedString Name;
        public LocalizedString Description;
        public bool Repeatable = false;
        [SerializeReference] public List<IQuestRequirements> QuestRequirements;

        [Header("Steps")]
        public List<QuestStep> QuestSteps;

        [Header("Logic")]
        public AssetReferenceGameObject Logic;

        public int GetStepReference(string guid)
        {
            for (int i = 0; i < QuestSteps.Count; i++)
                if (guid == QuestSteps[i].GUID)
                    return i;

            return -1;
        }

        public string GetStepReference(int index)
        {
            return QuestSteps[index].GUID;
        }

        public QuestStep GetStep(string guid)
        {
            foreach (var step in QuestSteps)
                if (step.GUID == guid)
                    return step;

            return null;
        }

        public QuestStep GetStep(int index)
        {
            if (index < 0 || index > QuestSteps.Count)
                return null;

            return QuestSteps[index];
        }

        public QuestGoal GetGoal(Goal goal)
        {
            var step = GetStep(goal.StepID);

            foreach (var stepGoal in step.Goals)
                if (stepGoal.GUID == goal.GoalID)
                    return stepGoal;

            return null;
        }

        public bool RequirementsAreMet()
        {
            foreach (var requirement in QuestRequirements)
            {
                if (!requirement.RequirementCompleted()) return false;
            }
            return true;
        }

        public async Task<QuestLogic> StartQuest(CancellationTokenSource cancellationToken, QuestData data)
        {
            var op = Addressables.InstantiateAsync(Logic);

            while (!op.IsDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Addressables.Release(op);
                    throw new OperationCanceledException();
                }

                //16ms = 1 frame at 60 fps
                await Task.Delay(16);
            }

            QuestLogic questLogic = op.Result.GetComponent<QuestLogic>();

            DontDestroyOnLoad(questLogic.gameObject);
            questLogic.Initialize(data);

            return questLogic;
        }
    }
    public interface IQuestRequirements
    {
        bool RequirementCompleted();
    }
}