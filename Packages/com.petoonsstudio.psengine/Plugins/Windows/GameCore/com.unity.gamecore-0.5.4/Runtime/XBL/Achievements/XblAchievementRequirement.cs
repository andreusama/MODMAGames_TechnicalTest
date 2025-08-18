namespace Unity.GameCore
{
    public class XblAchievementRequirement
    {
        internal XblAchievementRequirement(Interop.XblAchievementRequirement interopRequirement)
        {
            this.Id = interopRequirement.id.GetString();
            this.CurrentProgressValue = interopRequirement.currentProgressValue.GetString();
            this.TargetProgressValue = interopRequirement.targetProgressValue.GetString();
        }

        public string Id { get; }
        public string CurrentProgressValue { get; }
        public string TargetProgressValue { get; }
    }
}
