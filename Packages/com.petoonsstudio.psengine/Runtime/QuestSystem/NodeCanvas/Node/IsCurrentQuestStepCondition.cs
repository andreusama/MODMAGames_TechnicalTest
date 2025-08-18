using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    [Category("Dialogue/Conditions")]
    public class IsCurrentQuestStepCondition : ConditionTask
    {
        public BBParameter<StepComparer> State;
        [StepNodeCanvas] 
        public StepNodeData StepNode = new StepNodeData();


        public enum StepComparer
        {
            At,
            NotAt,
            Before,
            BeforeOrAt,
            After,
            AfterOrAt
        }

        protected override bool OnCheck()
        {
            if (QuestSystem.Instance == null)
                return false;

            if (!QuestSystem.Instance.IsQuestActive(StepNode.Quest))
                return false;

            switch (State.value)
            {
                case StepComparer.At:
                    return QuestSystem.Instance.IsQuestAtGivenStep(StepNode.Quest, StepNode.Step);
                case StepComparer.NotAt:
                    return !QuestSystem.Instance.IsQuestAtGivenStep(StepNode.Quest, StepNode.Step);
                case StepComparer.Before:
                    return QuestSystem.Instance.IsQuestAtLesserStep(StepNode.Quest, StepNode.Step);
                case StepComparer.BeforeOrAt:
                    return QuestSystem.Instance.IsQuestAtLesserOrEqualStep(StepNode.Quest, StepNode.Step);
                case StepComparer.After:
                    return QuestSystem.Instance.IsQuestAtGreaterStep(StepNode.Quest, StepNode.Step);
                case StepComparer.AfterOrAt:
                    return QuestSystem.Instance.IsQuestAtGreaterOrEqualStep(StepNode.Quest, StepNode.Step);
                default:
                    return false;
            }
        }

        protected override string info
        {
            get 
            {
                string description = "Is ";

                description += $"{State.value} step ";


#if UNITY_EDITOR
                if (StepNode != null && !string.IsNullOrEmpty(StepNode.Step))
                {
                    var step = QuestSystemTools.FindStep(StepNode.Step, out QuestData quest);
                    if (step != null)
                        description += $"{quest.GetStepReference(step.GUID)}: {QuestSystemTools.GenerateStepDescription(step, 30, 20)} ";
                }

#else
                description += $"{StepNode.Step} ";
#endif
                description += $"on {StepNode.Quest}";

                return description;
            }
        }
    }
}
