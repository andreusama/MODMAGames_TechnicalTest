using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Category("Dialogue/Conditions")]
    public class IsQuestCondition : ConditionTask
    {
        [QuestNodeCanvas]
        public string QuestID;

        public BBParameter<QuestState> State;

        public enum QuestState
        {
            Available,
            Completed,
            Active
        }

        protected override bool OnCheck()
        {
            if (QuestSystem.Instance == null)
                return false;

            switch (State.value)
            {
                case QuestState.Available:
                    return QuestSystem.Instance.IsQuestAvailable(QuestID);
                case QuestState.Completed:
                    return QuestSystem.Instance.IsQuestCompleted(QuestID);
                case QuestState.Active:
                    return QuestSystem.Instance.IsQuestActive(QuestID);
                default:
                    return false;
            }
        }

        protected override string info
        {
            get 
            {
                return $"Is quest {QuestID} {State}"; 
            }
        }
    }
}