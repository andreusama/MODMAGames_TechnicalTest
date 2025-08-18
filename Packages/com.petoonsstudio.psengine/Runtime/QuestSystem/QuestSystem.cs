using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public struct PSCompleteQuestEvent
    {
        public string QuestID;

        public PSCompleteQuestEvent(string questID)
        {
            QuestID = questID;
        }
    }

    public struct PSStartQuestEvent
    {
        public string QuestID;

        public PSStartQuestEvent(string questID)
        {
            QuestID = questID;
        }
    }

    public struct PSCancelQuestEvent
    {
        public string QuestID;

        public PSCancelQuestEvent(string questID)
        {
            QuestID = questID;
        }
    }

    public struct PSUpdateQuestEvent
    {
        public string QuestID;

        public PSUpdateQuestEvent(string questID)
        {
            QuestID = questID;
        }
    }

    public class QuestSystem
    {
        protected Dictionary<string, QuestLogic> m_ActiveQuests;
        protected List<string> m_CompletedQuests;

        protected int m_MaxBufferSize = 5;
        protected float m_CacheUpdateInterval = 0.5f;
        protected CancellationTokenSource m_CancellationTokenSource;

        public Dictionary<string, QuestLogic> ActiveQuests => m_ActiveQuests;
        public List<string> CompletedQuests => m_CompletedQuests;

        public bool IsEmpty => m_CompletedQuests.Count == 0 && m_ActiveQuests.Count == 0;

        public static QuestSystem Instance;

        public QuestSystem()
        {
            m_ActiveQuests = new Dictionary<string, QuestLogic>();
            m_CompletedQuests = new List<string>();

            Instance = this;
        }

        protected QuestData LoadAsset(string id)
        {
            return QuestDB.LoadAsset(id);
        }

        public void ReleaseAsset(QuestData asset)
        {
            if (!IsQuestActive(asset.ID))
            {
                QuestDB.ReleaseAsset(asset);
            }
        }

        public QuestData GetQuestAsset(string questID)
        {
            if (IsQuestActive(questID))
                return m_ActiveQuests[questID].QuestData;

            return LoadAsset(questID);
        }

        public bool IsQuestActive(string questID)
        {
            if (m_ActiveQuests == null)
                return false;

            return m_ActiveQuests.ContainsKey(questID);
        }
        public bool IsQuestCompleted(string questID)
        {
            if (m_CompletedQuests == null)
                return false;

            return m_CompletedQuests.Contains(questID);
        }
        public virtual bool IsQuestAvailable(string questID, bool ignoreRequirements = false)
        {
            if (m_ActiveQuests.ContainsKey(questID))
                return false;

            var quest = GetQuestAsset(questID);

            if (m_CompletedQuests.Contains(questID) && !quest.Repeatable)
                return false;

            if (ignoreRequirements)
                return true;

            bool requirements = quest.RequirementsAreMet();

            ReleaseAsset(quest);

            return requirements;
        }

        public bool IsQuestAtGivenStep(string questID, int step)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestAtGivenStep(step);
        }
        public bool IsQuestAtGivenStep(string questID, string stepGUID)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestAtGivenStep(stepGUID);
        }
        public bool IsQuestStepCompleted(string questID, int step)
        {
            if (IsQuestCompleted(questID))
                return true;

            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestStepCompleted(step);
        }
        public bool IsQuestStepCompleted(string questID, string stepGUID)
        {
            if (IsQuestCompleted(questID))
                return true;

            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestStepCompleted(stepGUID);
        }
        public bool IsQuestAtGreaterStep(string questID, string stepGUID)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestAtGreaterStep(stepGUID);
        }
        public bool IsQuestAtGreaterOrEqualStep(string questID, string stepGUID)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestAtGreaterOrEqualStep(stepGUID);
        }
        public bool IsQuestAtLesserStep(string questID, string stepGUID)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestAtLesser(stepGUID);
        }
        public bool IsQuestAtLesserOrEqualStep(string questID, string stepGUID)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestAtLesserOrEqualStep(stepGUID);
        }
        public bool IsQuestGoalActive(string questID, string goalID)
        {
            if (!IsQuestActive(questID))
                return false;

            return m_ActiveQuests[questID].IsQuestGoalActive(goalID);
        }


        /// <summary>
        /// Accept a new quest
        /// </summary>
        /// <param name="quest"></param>
        public async virtual Task NewQuest(string ID, bool ignoreRequirements = false)
        {
            if (!IsQuestAvailable(ID, ignoreRequirements))
            {
                Debug.LogWarning($"Quest with id {ID} is already on the Active Quests dictionary, is already completed or it's requirements haven't been met.");
                return;
            }

            var quest = LoadAsset(ID);

            m_CancellationTokenSource = new CancellationTokenSource();

            try
            {
                var task = await quest.StartQuest(m_CancellationTokenSource, quest);
                m_ActiveQuests.Add(quest.ID, task);

                PSEventManager.TriggerEvent(new PSStartQuestEvent(quest.ID));
                task.StartQuest();
            }
            finally
            {
                m_CancellationTokenSource.Dispose();
                m_CancellationTokenSource = null;
            }
        }

        public void CancelQuestSystemAsyncOperations()
        {
            if (m_CancellationTokenSource != null)
                m_CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Abandon quest progress
        /// </summary>
        /// <param name="ID"></param>
        public virtual void CancelQuest(string ID)
        {
            if (!m_ActiveQuests.ContainsKey(ID))
                return;

            m_ActiveQuests[ID].CancelQuest();
            m_ActiveQuests.Remove(ID);

            PSEventManager.TriggerEvent(new PSCancelQuestEvent(ID));
        }

        /// <summary>
        /// Get ungranted step rewards and mark the quest as completed
        /// </summary>
        /// <param name="ID"></param>
        public virtual void CompleteQuest(string ID)
        {
            if (!m_ActiveQuests.ContainsKey(ID))
                return;

            m_ActiveQuests[ID].CompleteQuest();
        }

        /// <summary>
        /// Mark quest as completed
        /// </summary>
        /// <param name="quest"></param>
        public virtual void MarkQuestAsCompleted(string ID)
        {
            if (!m_ActiveQuests.ContainsKey(ID))
                return;

            m_ActiveQuests.Remove(ID);
            m_CompletedQuests.Add(ID);

            PSEventManager.TriggerEvent(new PSCompleteQuestEvent(ID));
        }

        /// <summary>
        /// Delete a quest from the completed quests list
        /// </summary>
        /// <param name="questID"></param>
        public virtual void DeleteCompletedQuest(string ID)
        {
            if (!IsQuestCompleted(ID))
            {
                Debug.LogWarning($"Quest with id {ID} is not on the Quest completed list");
                return;
            }

            m_CompletedQuests.Remove(ID);
        }

        #region Serialization

        public virtual async Task Deserialize(SerializedQuestSystem data)
        {
            m_CompletedQuests = data.CompletedQuests;

            foreach (var questID in data.ActiveQuests.Keys)
            {
                var quest = LoadAsset(questID);

                m_CancellationTokenSource = new CancellationTokenSource();

                try
                {
                    var logic = await quest.StartQuest(m_CancellationTokenSource, quest);

                    m_ActiveQuests.Add(quest.ID, logic);
                    logic.Deserialize(data.ActiveQuests[questID]);
                }
                finally
                {
                    m_CancellationTokenSource.Dispose();
                    m_CancellationTokenSource = null;
                }
            }
        }

        #endregion
    }

    public class SerializedQuestSystem
    {
        public Dictionary<string, SerializedQuestLogic> ActiveQuests;
        public List<string> CompletedQuests;

        public SerializedQuestSystem() { }

        public SerializedQuestSystem(QuestSystem current)
        {
            CompletedQuests = new List<string>();
            ActiveQuests = new Dictionary<string, SerializedQuestLogic>();

            foreach (var quest in current.CompletedQuests)
                CompletedQuests.Add(quest);

            foreach (var quest in current.ActiveQuests.Keys)
                ActiveQuests.Add(quest, current.ActiveQuests[quest].Serialize());
        }
    }
}