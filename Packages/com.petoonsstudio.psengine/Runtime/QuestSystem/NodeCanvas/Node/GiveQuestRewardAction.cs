namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class GiveQuestRewardAction : GiveRewardAction
    {
        [QuestNodeCanvas]
        public string QuestID;

        protected override string info
        {
            get
            {
                var quest = QuestDB.LoadAsset(QuestID);
                var questID = quest != null ? quest.ID : string.Empty;
                var questName = quest != null ? quest.Name.GetLocalizedString() : string.Empty;
                string value = $"Give quest [{QuestID}] \"{questName}\"";
                return value;
            }
        }

        protected async override void GrantReward_Normal()
        {
            await QuestSystem.Instance.NewQuest(QuestID);
            EndAction(true);
        }
#if PETOONS_DEBUG
        protected async override void GrantReward_Debug()
        {
            await QuestSystem.Instance.NewQuest(QuestID);
            EndAction(true);
        }
#endif
    }
}
