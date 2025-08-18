using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Category("QuestSystem")]
    public abstract class QuestAction : ActionTask<QuestLogic>
    {
        protected override void OnExecute()
        {
            base.OnExecute();

#if PETOONS_DEBUG
            if(agent.DebugMode)
                OnExecute_Debug();
            else
#endif
                OnExecute_Normal();
        }

        protected abstract void OnExecute_Normal();
#if PETOONS_DEBUG
        protected abstract void OnExecute_Debug();
#endif
    }
}
