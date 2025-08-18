using ParadoxNotion.Design;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Category("QuestSystem/Rewards")]
    public abstract class GiveRewardAction : QuestAction
    {
        protected override void OnExecute_Normal()
        {
            GrantReward_Normal();
        }
#if PETOONS_DEBUG
        protected override void OnExecute_Debug()
        {
            GrantReward_Debug();
        }
#endif

        protected abstract void GrantReward_Normal();
#if PETOONS_DEBUG
        protected abstract void GrantReward_Debug();
#endif
    }
} 
