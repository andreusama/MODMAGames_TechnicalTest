using System;

namespace Unity.GameCore
{
    public class XblAchievementTimeWindow
    {
        internal XblAchievementTimeWindow(Interop.XblAchievementTimeWindow interopTimeWindow)
        {
            this.StartDate = interopTimeWindow.startDate.DateTime;
            this.EndDate = interopTimeWindow.endDate.DateTime;
        }

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}
