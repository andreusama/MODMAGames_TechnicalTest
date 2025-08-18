using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore
{
    public class XblAchievementTitleAssociation
    {
        internal XblAchievementTitleAssociation(Interop.XblAchievementTitleAssociation interopTitleAssociation)
        {
            this.Name = interopTitleAssociation.name.GetString();
            this.TitleId = interopTitleAssociation.titleId;
        }

        public string Name { get; }
        public UInt32 TitleId { get; }
    }
}
