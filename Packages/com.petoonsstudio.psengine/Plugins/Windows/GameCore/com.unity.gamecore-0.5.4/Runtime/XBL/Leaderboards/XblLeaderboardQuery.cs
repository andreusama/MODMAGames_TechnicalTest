using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblLeaderboardQuery
    {
        private XblLeaderboardQuery(
            UInt64 xboxUserId,
            string serviceConfigurationId,
            string leaderboardName,
            string statName,
            XblSocialGroupType socialGroup,
            string[] additionalColumnleaderboardNames,
            XblLeaderboardSortOrder order,
            UInt32 maxItems,
            UInt64 skipToXboxUserId,
            UInt32 skipResultToRank,
            string continuationToken
#if !(GDK_AUGUST_2020_OR_EARLIER)
            , XblLeaderboardQueryType queryType
#endif
            )
        {
            this.XboxUserId = xboxUserId;
            this.ServiceConfigurationId = serviceConfigurationId;
            this.LeaderboardName = leaderboardName;
            this.StatName = statName;
            this.SocialGroup = socialGroup;
            this.AdditionalColumnleaderboardNames = additionalColumnleaderboardNames;
            this.Order = order;
            this.MaxItems = maxItems;
            this.SkipToXboxUserId = skipToXboxUserId;
            this.SkipResultToRank = skipResultToRank;
            this.ContinuationToken = continuationToken;
#if !(GDK_AUGUST_2020_OR_EARLIER)
            this.QueryType = queryType;
#endif
        }

        internal XblLeaderboardQuery(Interop.XblLeaderboardQuery interopLeaderboardQuery)
        {
            this.XboxUserId = interopLeaderboardQuery.xboxUserId;
            this.ServiceConfigurationId = interopLeaderboardQuery.GetScid();
            this.LeaderboardName = interopLeaderboardQuery.leaderboardName.GetString();
            this.StatName = interopLeaderboardQuery.statName.GetString();
            this.SocialGroup = interopLeaderboardQuery.socialGroup;
            this.AdditionalColumnleaderboardNames = interopLeaderboardQuery.GetAdditionalColumnleaderboardNames();
            this.Order = interopLeaderboardQuery.order;
            this.MaxItems = interopLeaderboardQuery.maxItems;
            this.SkipToXboxUserId = interopLeaderboardQuery.skipToXboxUserId;
            this.SkipResultToRank = interopLeaderboardQuery.skipResultToRank;
            this.ContinuationToken = interopLeaderboardQuery.continuationToken.GetString();
#if !(GDK_AUGUST_2020_OR_EARLIER)
            this.QueryType = interopLeaderboardQuery.queryType;
#endif
        }

        public static Int32 Create(
            UInt64 xboxUserId,
            string serviceConfigurationId,
            string leaderboardName,
            string statName,
            XblSocialGroupType socialGroup,
            string[] additionalColumnleaderboardNames,
            XblLeaderboardSortOrder order,
            UInt32 maxItems,
            UInt64 skipToXboxUserId,
            UInt32 skipResultToRank,
            string continuationToken,
#if !(GDK_AUGUST_2020_OR_EARLIER)
            XblLeaderboardQueryType queryType,
#endif
            out XblLeaderboardQuery leaderboardQuery
            )
        {
            if (!Interop.XblLeaderboardQuery.ValidateFields(scid: serviceConfigurationId))
            {
                leaderboardQuery = default(XblLeaderboardQuery);
                return HR.E_INVALIDARG;
            }

            leaderboardQuery = new XblLeaderboardQuery(
                xboxUserId,
                serviceConfigurationId,
                leaderboardName,
                statName,
                socialGroup,
                additionalColumnleaderboardNames,
                order,
                maxItems,
                skipToXboxUserId,
                skipResultToRank,
                continuationToken
#if !(GDK_AUGUST_2020_OR_EARLIER)
                , queryType
#endif
                );

            return HR.S_OK;
        }

        public UInt64 XboxUserId { get; }
        public string ServiceConfigurationId { get; }
        public string LeaderboardName { get; }
        public string StatName { get; }
        public XblSocialGroupType SocialGroup { get; }
        public string[] AdditionalColumnleaderboardNames { get; }
        public XblLeaderboardSortOrder Order { get; }
        public UInt32 MaxItems { get; }
        public UInt64 SkipToXboxUserId { get; }
        public UInt32 SkipResultToRank { get; }
        public string ContinuationToken { get; }

#if !(GDK_AUGUST_2020_OR_EARLIER)
        public XblLeaderboardQueryType QueryType { get; }
#endif
    }
}
