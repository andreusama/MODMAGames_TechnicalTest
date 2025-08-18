using NodeCanvas.BehaviourTrees;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [RequireComponent(typeof(BehaviourTreeOwner))]
    public class QuestLogic : MonoBehaviour, ISerialize<SerializedQuestLogic>
    {
        [ReadOnly] public bool IsActive = false;

        [SerializeField, Quest] public string m_QuestID;

        [SerializeField]
        private BehaviourTreeOwner m_BehaviourOwner;

        [Header("Logic")]
        [SerializeReference] public List<QuestStepLogic> Steps = new List<QuestStepLogic>();

        protected int m_CurrentStep;
        public int CurrentStep => m_CurrentStep;

        public BehaviourTreeOwner BehaviourOwner => m_BehaviourOwner;

        public QuestData QuestData { get; protected set; }

        public Action OnQuestCompleted;

#if PETOONS_DEBUG
        public bool DebugMode { get; protected set; }
#endif

        public void Initialize(QuestData questData)
        {
            QuestData = questData;
            GenerateSteps();
        }

        protected virtual void GenerateSteps()
        {
            Steps.Clear();

            foreach (var step in QuestData.QuestSteps)
            {
                QuestStepLogic newStepLogic = new QuestStepLogic(step, this);

                foreach (var goal in step.Goals)
                    newStepLogic.LogicGoals.Add(goal.GenerateLogic(newStepLogic));

                Steps.Add(newStepLogic);
            }

            if (!isActiveAndEnabled)
                return;

            foreach (var step in Steps)
            {
                step.OnStepCompleted += OnStepCompleted;
            }
        }

        protected virtual void Awake()
        {
            if (m_BehaviourOwner == null)
                m_BehaviourOwner = GetComponent<BehaviourTreeOwner>();
        }

        protected virtual void OnEnable()
        {
            foreach (var step in Steps)
            {
                step.OnStepCompleted += OnStepCompleted;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var step in Steps)
            {
                step.OnStepCompleted -= OnStepCompleted;
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var step in Steps)
                step.Clear();
        }

        public void StartQuest()
        {
            StartCoroutine(Steps[m_CurrentStep].Start());
            IsActive = true;
        }

        #region HELPERS
        public bool IsQuestAtGivenStep(int step)
        {
            return CurrentStep == step;
        }
        public bool IsQuestAtGivenStep(string stepGUID)
        {
            return IsQuestAtGivenStep(QuestData.GetStepReference(stepGUID));
        }
        public bool IsQuestStepCompleted(int step)
        {
            return Steps[step].IsCompleted();
        }
        public bool IsQuestStepCompleted(string stepGUID)
        {
            return IsQuestStepCompleted(QuestData.GetStepReference(stepGUID));
        }
        public bool IsQuestAtGreaterStep(string stepGUID)
        {
            int stepId = QuestData.GetStepReference(stepGUID);
            return stepId < m_CurrentStep;
        }
        public bool IsQuestAtGreaterOrEqualStep(string stepGUID)
        {
            int stepId = QuestData.GetStepReference(stepGUID);
            return stepId <= m_CurrentStep;
        }
        public bool IsQuestAtLesser(string stepGUID)
        {
            int stepId = QuestData.GetStepReference(stepGUID);
            return stepId > m_CurrentStep;
        }
        public bool IsQuestAtLesserOrEqualStep(string stepGUID)
        {
            int stepId = QuestData.GetStepReference(stepGUID);
            return stepId >= m_CurrentStep;
        }

        public bool IsQuestGoalActive(string goalID)
        {
            return Steps[CurrentStep].IsGoalActive(goalID);
        }
        #endregion

        public SerializedQuestLogic Serialize()
        {
            return new SerializedQuestLogic(this);
        }

        public void Deserialize(SerializedQuestLogic data)
        {
            m_CurrentStep = data.CurrentStep;

            foreach (var step in Steps)
            {
                foreach (var goal in step.LogicGoals)
                {
                    goal.Deserialize(data.Goals[goal.Goal.GUID]);
                }
            }
        }

        public virtual void RestartQuest()
        {
            Steps[m_CurrentStep].Restart();
            IsActive = true;
        }

        protected virtual void OnStepCompleted()
        {
            m_CurrentStep++;

            if (CurrentStep > Steps.Count - 1)
            {
                QuestCompleted();
            }
            else
                StartNextStep();
        }

        protected virtual void QuestCompleted()
        {
            QuestSystem.Instance.MarkQuestAsCompleted(m_QuestID);

            OnQuestCompleted?.Invoke();

            Destroy(gameObject);
        }

#if PETOONS_DEBUG
        public virtual void CompleteStep_Debug()
        {
            StartCoroutine(CompleteStep_Debug_Internal());
        }
        public virtual void CompleteQuest_Debug()
        {
            StartCoroutine(CompleteQuest_Debug_Internal());
        }

        protected virtual IEnumerator CompleteStep_Debug_Internal()
        {
            DebugMode = true;

            yield return Steps[m_CurrentStep].End_Debug();

            m_CurrentStep++;

            if (CurrentStep > Steps.Count - 1)
            {
                QuestSystem.Instance.MarkQuestAsCompleted(m_QuestID);

                Destroy(gameObject);
            }
            else if (CurrentStep < Steps.Count - 1)
            {
                Steps[m_CurrentStep].Start_Debug();
            }

            PSEventManager.TriggerEvent(new PSUpdateQuestEvent(m_QuestID));

            DebugMode = false;
        }
        protected virtual IEnumerator CompleteQuest_Debug_Internal()
        {
            do
                yield return CompleteStep_Debug_Internal();
            while (m_CurrentStep <= Steps.Count - 1);
        }
#endif

        protected virtual void StartNextStep()
        {
            StartCoroutine(StartNextStep_Internal());
        }
        protected virtual IEnumerator StartNextStep_Internal()
        {
            yield return Steps[m_CurrentStep].Start();

            PSEventManager.TriggerEvent(new PSUpdateQuestEvent(m_QuestID));
        }

        public virtual void CompleteQuest()
        {
            QuestCompleted();
        }

        public virtual void CancelQuest()
        {
            Destroy(gameObject);
        }

        public virtual QuestGoalLogic GetCurrentUncompletedGoal()
        {
            var currentStep = Steps[m_CurrentStep];

            foreach (var goal in currentStep.LogicGoals)
            {
                if (!goal.Goal.IsOptional && !goal.IsCompleted)
                    return goal;
            }

            return null;
        }
    }

    [System.Serializable]
    public class SerializedQuestLogic
    {
        public Dictionary<string, SerializedQuestGoalLogic> Goals;
        public int CurrentStep;

        public SerializedQuestLogic()
        {
            Goals = new Dictionary<string, SerializedQuestGoalLogic>();
            CurrentStep = 0;
        }

        public SerializedQuestLogic(QuestLogic logic)
        {
            Goals = new Dictionary<string, SerializedQuestGoalLogic>();

            foreach (var step in logic.Steps)
            {
                foreach (var goal in step.LogicGoals)
                {
                    Goals.Add(goal.Goal.GUID, goal.Serialize());
                }
            }
            CurrentStep = logic.CurrentStep;
        }
    }
}