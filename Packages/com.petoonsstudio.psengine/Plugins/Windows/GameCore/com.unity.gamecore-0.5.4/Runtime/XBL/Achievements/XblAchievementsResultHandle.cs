using System;

namespace Unity.GameCore
{
    public class XblAchievementsResultHandle
    {
        internal XblAchievementsResultHandle(Interop.XblAchievementsResultHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal Interop.XblAchievementsResultHandle InteropHandle { get; set; }
    }
}
