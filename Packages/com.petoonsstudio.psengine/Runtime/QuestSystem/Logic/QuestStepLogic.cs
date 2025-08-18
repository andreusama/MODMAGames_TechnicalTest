using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public delegate void StepDelegate();

    [Serializable]
    public class QuestStepLogic
    {
        [SerializeReference] public QuestStep Step;
        [SerializeReference] public QuestLogic QuestLogic;

        public event StepDelegate OnStepStarted;
        public event StepDelegate OnStepCompleted;

        [SerializeReference] protected List<QuestGoalLogic> m_LogicGoals;

        public List<QuestGoalLogic> LogicGoals => m_LogicGoals;

        public QuestStepLogic(QuestStep step, QuestLogic logic)
        {
            Step = step;
            QuestLogic = logic;

            m_LogicGoals = new List<QuestGoalLogic>();
        }

        public virtual IEnumerator Start()
        {
            yield return ExecutePreActions();

            StartGoals();

            OnStepStarted?.Invoke();
        }

        public virtual void Restart()
        {
            StartGoals();
        }

        public virtual IEnumerator End()
        {
            Clear();

            yield return ExecutePostActions();

            OnStepCompleted?.Invoke();
        }

        protected virtual IEnumerator ExecutePreActions()
        {
            if (Step.PreBehaviour != null)
            {
                QuestLogic.BehaviourOwner.behaviour = Step.PreBehaviour;
                QuestLogic.BehaviourOwner.repeat = false;
                QuestLogic.BehaviourOwner.StartBehaviour();

                yield return new WaitWhile(() => QuestLogic.BehaviourOwner.isRunning);

                QuestLogic.BehaviourOwner.behaviour = null;
            }
        }
        protected virtual IEnumerator ExecutePostActions()
        {
            if (Step.PostBehaviour != null)
            {
                QuestLogic.BehaviourOwner.behaviour = Step.PostBehaviour;
                QuestLogic.BehaviourOwner.repeat = false;
                QuestLogic.BehaviourOwner.StartBehaviour();

                yield return new WaitWhile(() => QuestLogic.BehaviourOwner.isRunning);

                QuestLogic.BehaviourOwner.behaviour = null;
            }
        }

        protected virtual void StartGoals()
        {
            foreach (var goal in m_LogicGoals)
            {
                goal.OnGoalComplete += OnGoalComplete;
                goal.Start();
            }
        }

        public virtual void Clear(bool forceCompleted = false)
        {
            foreach (var goal in m_LogicGoals)
            {
                if (forceCompleted)
                    goal.IsCompleted = true;
                goal.OnGoalComplete -= OnGoalComplete;
                goal.End();
            }
        }

        public bool IsGoalCompleted(string goalGUID)
        {
            foreach (var goal in m_LogicGoals)
                if (goal.Goal.GUID == goalGUID && goal.IsCompleted)
                    return true;

            return false;
        }

        public bool IsGoalActive(string goalGUID)
        {
            foreach (var goal in m_LogicGoals)
                if (goal.Goal.GUID == goalGUID && !goal.IsCompleted)
                    return true;

            return false;
        }

        public bool IsCompleted()
        {
            foreach (var goal in m_LogicGoals)
                if (!goal.Goal.IsOptional && !goal.IsCompleted)
                    return false;

            return true;
        }

        public void OnGoalComplete()
        {
            if (IsCompleted())
            {
                QuestLogic.StartCoroutine(End());
            }
        }

#if PETOONS_DEBUG
        public IEnumerator Start_Debug()
        {
            yield return ExecutePreActions();

            Restart();
        }

        public IEnumerator End_Debug()
        {
            yield return CompleteGoals_Debug();

            Clear(true);

            yield return ExecutePostActions();

            OnStepCompleted?.Invoke();
        }

        protected IEnumerator CompleteGoals_Debug()
        {
            foreach (var goal in m_LogicGoals)
                yield return goal.CompleteGoal_Debug();
        }
#endif
    }
}