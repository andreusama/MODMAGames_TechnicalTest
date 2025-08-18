using System;

namespace Unity.GameCore
{
    public class XblLeaderboardRow
    {
        internal XblLeaderboardRow(Interop.XblLeaderboardRow interopRow)
        {
            this.Gamertag = Interop.Converters.ByteArrayToString(interopRow.gamertag);
            this.ModernGamertag = Interop.Converters.ByteArrayToString(interopRow.modernGamertag);
            this.ModernGamertagSuffix = Interop.Converters.ByteArrayToString(interopRow.modernGamertagSuffix);
            this.UniqueModernGamertag = Interop.Converters.ByteArrayToString(interopRow.uniqueModernGamertag);
            this.XboxUserId = interopRow.xboxUserId;
            this.Percentile = interopRow.percentile;
            this.Rank = interopRow.rank;
            this.ColumnValues = interopRow.GetColumnValues();
        }

        public string Gamertag { get; }
        public string ModernGamertag { get; }
        public string ModernGamertagSuffix { get; }
        public string UniqueModernGamertag { get; }
        public UInt64 XboxUserId { get; }
        public double Percentile { get; }
        public UInt32 Rank { get; }
        public string[] ColumnValues { get; }
    }
}
