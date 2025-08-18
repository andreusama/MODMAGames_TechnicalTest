#pragma once

#include "../PlayerInterface/UnityEventQueue.h"
#include <system_service.h>

// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_ON_RESUME.
struct PzazzOnResume { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xD725C2DB79674D8CULL, 0xA52A1009670D0880ULL, PzazzOnResume)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_GAME_LIVE_STREAMING_STATUS_UPDATE.
struct PzazzOnGameLiveStreamingStatusUpdate { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x11C8947A2EF44E45ULL, 0xA1F77F0AC927272EULL, PzazzOnGameLiveStreamingStatusUpdate)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_SESSION_INVITATION.
struct PzazzOnSessionInvitation { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xCF791F68B6884AEFULL, 0x8198EC6D98D70F8AULL, PzazzOnSessionInvitation)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_ENTITLEMENT_UPDATE.
struct PzazzOnEntitlementUpdate { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xDE76F015C0DE4BE8ULL, 0x9046B1153C877E39ULL, PzazzOnEntitlementUpdate)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_GAME_CUSTOM_DATA.
struct PzazzOnGameCustomData { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x7D073AAAF3004C2BULL, 0x810A278660A015D6ULL, PzazzOnGameCustomData)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_DISPLAY_SAFE_AREA_UPDATE.
struct PzazzOnDisplaySafeAreaUpdate { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xD69939D64B2544ABULL, 0xB5E6A5B6BC273194ULL, PzazzOnDisplaySafeAreaUpdate)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_URL_OPEN.
struct PzazzOnUrlOpen { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xC89E302816C644DBULL, 0x832F32CD9410F333ULL, PzazzOnUrlOpen)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_LAUNCH_APP.
struct PzazzOnLaunchApp { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xA1458D5B05264EA2ULL, 0xA70506FE4FCD11F3ULL, PzazzOnLaunchApp)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_APP_LAUNCH_LINK.
struct PzazzOnLaunchLink { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x477AFB5C1CA045D6ULL, 0x95E9C61B8365A66AULL, PzazzOnLaunchLink)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_ADDCONTENT_INSTALL.
struct PzazzOnAddcontentInstall { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x01E30CD6D6764564ULL, 0x9F2D243AC685D381ULL, PzazzOnAddcontentInstall)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_RESET_VR_POSITION.
struct PzazzOnResetVrPosition { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x2BD3588AC2A34A03ULL, 0x8401C671EAB0964BULL, PzazzOnResetVrPosition)
// ============================================================
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_JOIN_EVENT.
struct PzazzOnJoinEvent { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x8ABE8C89A5AD4C65ULL, 0xA818D17C31FD215EULL, PzazzOnJoinEvent)
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_PLAYGO_LOCUS_UPDATE.
struct PzazzOnPlaygoLocusUpdate { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x21A78E6026A443FBULL, 0xB4E9B96B3B18FDA0ULL, PzazzOnPlaygoLocusUpdate)
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_OPEN_SHARE_MENU.
struct PzazzOnOpenShareMenu { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xB7AD0B23F68B4361ULL, 0x802C36342FF448A7ULL, PzazzOnOpenShareMenu)
// This event is triggered for SCE_SYSTEM_SERVICE_EVENT_PLAY_TOGETHER_HOST.
struct PzazzOnPlayTogetherHost { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0xA136D11F66DB9F7FULL, 0x83EC5A5608DDDB55ULL, PzazzOnPlayTogetherHost)

struct PzazzOnSystemEvent { SceSystemServiceEvent params; };
REGISTER_EVENT_ID(0x926962B0FC4940C5ULL, 0x81DCC522B0823744ULL, PzazzOnSystemEvent)
