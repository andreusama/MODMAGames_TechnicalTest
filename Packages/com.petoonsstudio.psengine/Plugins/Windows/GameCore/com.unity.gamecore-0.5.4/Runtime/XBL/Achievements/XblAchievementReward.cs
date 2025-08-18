namespace Unity.GameCore
{
    public class XblAchievementReward
    {
        internal XblAchievementReward(Interop.XblAchievementReward interopReward)
        {
            this.Name = interopReward.name.GetString();
            this.Description = interopReward.description.GetString();
            this.Value = interopReward.value.GetString();
            this.RewardType = interopReward.rewardType;
            this.ValueType = interopReward.valueType.GetString();
            this.MediaAsset = interopReward.GetMediaAsset(ma => new XblAchievementMediaAsset(ma));
        }

        public string Name { get; }
        public string Description { get; }
        public string Value { get; }
        public XblAchievementRewardType RewardType { get; }
        public string ValueType { get; }
        public XblAchievementMediaAsset MediaAsset { get; }
    }
}
