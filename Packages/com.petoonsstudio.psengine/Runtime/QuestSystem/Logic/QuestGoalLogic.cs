using PetoonsStudio.PSEngine.Framework;
using System.Collections;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public delegate void GoalDelegate();
    public delegate IEnumerator GoalActionsDelegate();

    [System.Serializable]
    public abstract class QuestGoalLogic : ISerialize<SerializedQuestGoalLogic>
    {
        [SerializeReference] public QuestGoal Goal;
        [SerializeReference] public QuestStepLogic Step;

        public event GoalDelegate OnGoalComplete;

        public bool IsCompleted;

        public virtual void Start()
        {
            if (AlreadyCompleted())
                ReCompleteGoal();
        }
        public virtual void End() { }
        public virtual bool AlreadyCompleted() { return false; }


        public QuestGoalLogic(QuestGoal goal, QuestStepLogic step)
        {
            Goal = goal;
            Step = step;
        }

        public virtual void CompleteGoal()
        {
            if (IsCompleted)
                return;

            Step.QuestLogic.StartCoroutine(CompleteGoal_Internal());
        }

        protected virtual void ReCompleteGoal()
        {
            IsCompleted = true;
            OnGoalComplete?.Invoke();
        }

        protected virtual IEnumerator CompleteGoal_Internal()
        {
            yield return ExecutePostActions();

            IsCompleted = true;

            OnGoalComplete?.Invoke();
        }

        protected virtual IEnumerator ExecutePostActions()
        {
            if (Goal.PostActions != null)
            {
                Step.QuestLogic.BehaviourOwner.behaviour = Goal.PostActions;
                Step.QuestLogic.BehaviourOwner.repeat = false;
                Step.QuestLogic.BehaviourOwner.StartBehaviour();

                yield return new WaitWhile(() => Step.QuestLogic.BehaviourOwner.isRunning);

                Step.QuestLogic.BehaviourOwner.behaviour = null;
            }
        }

#if PETOONS_DEBUG
        public virtual IEnumerator CompleteGoal_Debug()
        {
            if (IsCompleted)
                yield break;

            yield return ExecutePostActions();

            IsCompleted = true;

            OnGoalComplete?.Invoke();
        }
#endif

        public abstract SerializedQuestGoalLogic Serialize();
        public abstract void Deserialize(SerializedQuestGoalLogic data);
    }

    public class SerializedQuestGoalLogic
    {
        public bool IsCompleted;

        public SerializedQuestGoalLogic(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }
    }
}