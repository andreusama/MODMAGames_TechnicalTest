using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblTournamentTeamResult
    {
        internal XblTournamentTeamResult(Interop.XblTournamentTeamResult interopStruct)
        {
            this.Team = interopStruct.Team.GetString();
            this.GameResult = new XblTournamentGameResultWithRank(interopStruct.GameResult);
        }

        public string Team { get; }
        public XblTournamentGameResultWithRank GameResult { get; }
    }
}