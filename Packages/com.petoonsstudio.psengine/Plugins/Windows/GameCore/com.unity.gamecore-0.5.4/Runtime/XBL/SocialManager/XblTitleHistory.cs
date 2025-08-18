using System;

namespace Unity.GameCore
{
    public class XblTitleHistory
    {
        internal XblTitleHistory(Interop.XblTitleHistory interopTitleHistory)
        {
            this.HasUserPlayed = interopTitleHistory.hasUserPlayed;
            this.LastTimeUserPlayed = interopTitleHistory.lastTimeUserPlayed.DateTime;
        }

        public bool HasUserPlayed { get; }
        public DateTime LastTimeUserPlayed { get; }
    }
}
