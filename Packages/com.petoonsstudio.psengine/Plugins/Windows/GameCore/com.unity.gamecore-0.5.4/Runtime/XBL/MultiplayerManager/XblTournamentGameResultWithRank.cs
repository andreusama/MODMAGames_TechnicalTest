using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblTournamentGameResultWithRank
    {
        internal XblTournamentGameResultWithRank(Interop.XblTournamentGameResultWithRank interopStruct)
        {
            this.Result = interopStruct.Result;
            this.Ranking = interopStruct.Ranking;
        }

        public XblTournamentGameResult Result { get; }
        public UInt64 Ranking { get; }
    }
}