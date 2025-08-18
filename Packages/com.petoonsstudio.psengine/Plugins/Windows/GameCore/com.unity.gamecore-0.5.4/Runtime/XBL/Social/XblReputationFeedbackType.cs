﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    //enum class XblReputationFeedbackType : uint32_t
    //{
    //    FairPlayKillsTeammates,
    //    FairPlayCheater,
    //    FairPlayTampering,
    //    FairPlayQuitter,
    //    FairPlayKicked,
    //    CommunicationsInappropriateVideo,
    //    CommunicationsAbusiveVoice,
    //    InappropriateUserGeneratedContent,
    //    PositiveSkilledPlayer,
    //    PositiveHelpfulPlayer,
    //    PositiveHighQualityUserGeneratedContent,
    //    CommsPhishing,
    //    CommsPictureMessage,
    //    CommsSpam,
    //    CommsTextMessage,
    //    CommsVoiceMessage,
    //    FairPlayConsoleBanRequest,
    //    FairPlayIdler,
    //    FairPlayUserBanRequest,
    //    UserContentGamerpic,
    //    UserContentPersonalInfo,
    //    FairPlayUnsporting,
    //    FairPlayLeaderboardCheater
    //};

    public enum XblReputationFeedbackType : UInt32
    {
        FairPlayKillsTeammates = 0,
        FairPlayCheater,
        FairPlayTampering,
        FairPlayQuitter,
        FairPlayKicked,
        CommunicationsInappropriateVideo,
        CommunicationsAbusiveVoice,
        InappropriateUserGeneratedContent,
        PositiveSkilledPlayer,
        PositiveHelpfulPlayer,
        PositiveHighQualityUserGeneratedContent,
        CommsPhishing,
        CommsPictureMessage,
        CommsSpam,
        CommsTextMessage,
        CommsVoiceMessage,
        FairPlayConsoleBanRequest,
        FairPlayIdler,
        FairPlayUserBanRequest,
        UserContentGamerpic,
        UserContentPersonalInfo,
        FairPlayUnsporting,
        FairPlayLeaderboardCheater
    }
}
