using System;

namespace Unity.GameCore
{
    public class XblAchievementProgression
    {
        internal XblAchievementProgression(Interop.XblAchievementProgression interopProgression)
        {
            this.Requirements = interopProgression.GetRequirements(r => new XblAchievementRequirement(r));
            this.TimeUnlocked = interopProgression.timeUnlocked.DateTime;
        }

        public XblAchievementRequirement[] Requirements { get; }
        public DateTime TimeUnlocked { get; }
    }
}
