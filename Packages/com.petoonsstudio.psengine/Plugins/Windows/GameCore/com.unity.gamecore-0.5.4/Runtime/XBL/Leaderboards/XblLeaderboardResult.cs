using System;

namespace Unity.GameCore
{
    public class XblLeaderboardResult
    {
        internal XblLeaderboardResult(Interop.XblLeaderboardResult interopResult)
        {
            this.TotalRowCount = interopResult.totalRowCount;
            this.Columns = interopResult.GetColumns(c => new XblLeaderboardColumn(c));
            this.Rows = interopResult.GetRows(r => new XblLeaderboardRow(r));
            this.HasNext = interopResult.hasNext.Value;
            this.NextQuery = new XblLeaderboardQuery(interopResult.nextQuery);
        }

        public UInt32 TotalRowCount { get; }
        public XblLeaderboardColumn[] Columns { get; }
        public XblLeaderboardRow[] Rows { get; }
        public bool HasNext { get; }
        public XblLeaderboardQuery NextQuery { get; }
    }
}
