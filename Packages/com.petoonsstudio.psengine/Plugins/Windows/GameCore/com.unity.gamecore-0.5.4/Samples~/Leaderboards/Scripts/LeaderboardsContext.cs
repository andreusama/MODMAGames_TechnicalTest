using Unity.GameCore;

public delegate void LeaderboardGetResultAvailable(int hresult, uint pageNumber, XblLeaderboardResult xblLeaderboardResult);
public delegate void StatGetResultAvailable(int hresult, XblUserStatisticsResult xblStatResult);

namespace LeaderboardSample
{
    public class LeaderboardsContext
    {
        public const uint MaxItemsPerQuery = 5; // Deliberately set low in this sample to demonstrate querying multiple pages

        public uint Page { get; private set; }

        bool RequestInProgress { get; set; }

        ulong Xuid { get; }
        XblContextHandle ContextHandle { get; }

        LeaderboardGetResultAvailable LeaderboardCompletionRoutine { get; set; }
        StatGetResultAvailable StatCompletionRoutine { get; set; }

        public LeaderboardsContext(ulong xuid, XblContextHandle contextHandle)
        {
            Page = 0;
            Xuid = xuid;
            ContextHandle = contextHandle;
        }

        public void QueryLeaderboards(
            string leaderboardName,
            string statName,
            XblSocialGroupType queryGroupType,
            string[] additionalColumns,
            LeaderboardGetResultAvailable completionRoutine
            )
        {
            if (RequestInProgress)
            {
                completionRoutine(-1, 0, null);
                return;
            }

            RequestInProgress = true;
            LeaderboardCompletionRoutine = completionRoutine;

#if UNITY_GAMECORE
            XblLeaderboardQuery query;
            int hresult = XblLeaderboardQuery.Create(
                Xuid,
                UnityEngine.GameCore.GameCoreSettings.SCID,
                leaderboardName,
                statName,
                XblSocialGroupType.None,
                additionalColumns,
                XblLeaderboardSortOrder.Descending,
                MaxItemsPerQuery,
                0,
                0,
                null,
                XblLeaderboardQueryType.UserStatBacked,
                out query
                );

            if (HR.FAILED(hresult))
            {
                completionRoutine(hresult, 0, null);
            }

            SDK.XBL.XblLeaderboardGetLeaderboardAsync(
                ContextHandle,
                query,
                ProcessLeaderboardResult);
#endif //UNITY_GAMECORE
        }

        public void QueryStatistics(
        string statName,
        StatGetResultAvailable completionRoutine
        )
        {
            if (RequestInProgress)
            {
                completionRoutine(-1, null);
                return;
            }

            RequestInProgress = true;
            StatCompletionRoutine = completionRoutine;
#if UNITY_GAMECORE
            SDK.XBL.XblUserStatisticsGetSingleUserStatisticAsync(
                ContextHandle,
                Xuid,
                UnityEngine.GameCore.GameCoreSettings.SCID,
                statName,
                ProcessStatsResult);
#endif //UNITY_GAMECORE
        }

        public void ProcessLeaderboardResult(int hr, XblLeaderboardResult xblLeaderboardResult)
        {
            if (HR.SUCCEEDED(hr))
            {
                LeaderboardCompletionRoutine(hr, Page, xblLeaderboardResult);
                Page++;

                if (xblLeaderboardResult.HasNext)
                {
                    SDK.XBL.XblLeaderboardResultGetNextAsync(ContextHandle, xblLeaderboardResult, MaxItemsPerQuery, ProcessLeaderboardResult);
                }
                else
                {
                    RequestInProgress = false;
                    LeaderboardCompletionRoutine = null;
                }
            }
        }

        public void ProcessStatsResult(int hr, XblUserStatisticsResult result)
        {
            if (HR.SUCCEEDED(hr))
            {
                StatCompletionRoutine(hr, result);

                RequestInProgress = false;
                StatCompletionRoutine = null;
            }
        }
    }
}
