using System;

namespace Unity.GameCore
{
    public class XblAchievement
    {
        internal XblAchievement(Interop.XblAchievement interopAchievement)
        {
            this.Id = interopAchievement.id.GetString();
            this.ServiceConfigurationId = interopAchievement.serviceConfigurationId.GetString();
            this.Name = interopAchievement.name.GetString();
            this.TitleAssociations = interopAchievement.GetTitleAssociations(ta => new XblAchievementTitleAssociation(ta));
            this.ProgressState = interopAchievement.progressState;
            this.Progression = new XblAchievementProgression(interopAchievement.progression);
            this.MediaAssets = interopAchievement.GetMediaAssets(ma => new XblAchievementMediaAsset(ma));
            this.PlatformsAvailableOn = interopAchievement.GetPlatformsAvailableOn();
            this.IsSecret = interopAchievement.isSecret;
            this.UnlockedDescription = interopAchievement.unlockedDescription.GetString();
            this.LockedDescription = interopAchievement.lockedDescription.GetString();
            this.ProductId = interopAchievement.productId.GetString();
            this.Type = interopAchievement.type;
            this.ParticipationType = interopAchievement.participationType;
            this.Available = new XblAchievementTimeWindow(interopAchievement.available);
            this.Rewards = interopAchievement.GetRewards(reward => new XblAchievementReward(reward));
            this.EstimatedUnlockTime = interopAchievement.estimatedUnlockTime;
            this.DeepLink = interopAchievement.deepLink.GetString();
            this.IsRevoked = interopAchievement.isRevoked;
        }

        public string Id { get; }
        public string ServiceConfigurationId { get; }
        public string Name { get; }
        public XblAchievementTitleAssociation[] TitleAssociations { get; }
        public XblAchievementProgressState ProgressState { get; }
        public XblAchievementProgression Progression { get; }
        public XblAchievementMediaAsset[] MediaAssets { get; }
        public string[] PlatformsAvailableOn { get; }
        public bool IsSecret { get; }
        public string UnlockedDescription { get; }
        public string LockedDescription { get; }
        public string ProductId { get; }
        public XblAchievementType Type { get; }
        public XblAchievementParticipationType ParticipationType { get; }
        public XblAchievementTimeWindow Available { get; }
        public XblAchievementReward[] Rewards { get; }
        public UInt64 EstimatedUnlockTime { get; }
        public string DeepLink { get; }
        public bool IsRevoked { get; }
    }
}
