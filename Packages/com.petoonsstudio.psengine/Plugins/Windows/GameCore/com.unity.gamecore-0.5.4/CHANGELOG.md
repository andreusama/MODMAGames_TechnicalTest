# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.5.4] - 2022-05-11

* Added support for the following APIs:
    * XSpeechSynthesizerEnumerateInstalledVoices
    * XSpeechSynthesizerCreate
    * XSpeechSynthesizerCloseHandle
    * XSpeechSynthesizerSetDefaultVoice
    * XSpeechSynthesizerSetCustomVoice
    * XSpeechSynthesizerCreateStreamFromText
    * XSpeechSynthesizerCreateStreamFromSsml
    * XSpeechSynthesizerCloseStreamHandle
    * XSpeechSynthesizerGetStreamDataSize
    * XSpeechSynthesizerGetStreamData
    * XUserAddByIdWithUiAsync
    * XSystemGetAnalyticsInfo
    * XSystemGetConsoleId
    * XSystemGetXboxLiveSandboxId
    * XSystemGetAppSpecificDeviceId
* Added SpeechSynthesizer Sample

## [0.5.3] - 2022-02-08

* Added support for the following APIs:
    * XblMultiplayerSessionCurrentUserSetRoles
    * XblMultiplayerSessionCurrentUserSetEncounters
    * XblMultiplayerSessionCurrentUserSetMembersInGroup
    * XblMultiplayerSessionCurrentUserSetGroups
    * XblMultiplayerSessionCurrentUserSetCustomPropertyJson
    * XblMultiplayerSessionCurrentUserDeleteCustomPropertyJson
    * XblUserStatisticsAddStatisticChangedHandler
    * XblUserStatisticsRemoveStatisticChangedHandler
    * XblUserStatisticsStopTrackingStatistics
* Fixed Leaderboards sample retrieving the Maze leaderboard instead of the Cave one.
* Removed Unity logo from sample background picture.
* Users sample now runs on the same sandbox as the Leaderboards sample.
* Added hash defines so that sample code doesn't show errors in the Editor console on non-Game Core platforms.

## [0.5.2] - 2021-11-24

* Made the work port on the default async work queue run on a thread we create instead of using the system thread pool.
* Fixed a memory leak in HCWebsocketHandle.
* Fixed an issue where HCGetWebSocketSendMessageResult would contain a null websocket handle after a call to either HCWebSocketSendMessageAsync or HCWebSocketSendBinaryMessageAsync
* Added support for the following APIs:
    * HCInitialize
    * HCCleanupHandler
    * XblUserStatisticsTrackStatistics
    * XblMultiplayerMatchmakingServer
* The package is now signed

## [0.5.1] - 2021-10-20

* Exposed XGameSaveContainerInfo.NeedsSync
* Added support for the following APIs:
    * XblMultiplayerSessionTimeOfSession
    * XblMultiplayerSessionGetInitializationInfo
    * XblMultiplayerSessionSubscribedChangeTypes
    * XblMultiplayerSessionHostCandidates
    * XblMultiplayerSessionSessionConstants
    * XblMultiplayerSessionConstantsSetMaxMembersInSession
    * XblMultiplayerSessionConstantsSetVisibility
    * XblMultiplayerSessionConstantsSetTimeouts
    * XblMultiplayerSessionConstantsSetQosConnectivityMetrics
    * XblMultiplayerSessionConstantsSetMemberInitialization
    * XblMultiplayerSessionConstantsSetPeerToPeerRequirements
    * XblMultiplayerSessionConstantsSetMeasurementServerAddressesJson
    * XblMultiplayerSessionConstantsSetCapabilities


## [0.5.0] - 2021-09-16

    * The package now requires the April 2021 QFE 2 GDK (210402) or later because of additions to the GDK API, specifically the Controller API XUserFindControllerForUserWithUiAsync.
    * Added support for XUserFindControllerForUserWithUiAsync.
    * Added support for XblMultiplayerSessionSessionReference.
    * Fixed XblMultiplayerGetActivitiesWithPropertiesForSocialGroupAsync crashing when it finds 0 activities (case 1362151)

## [0.4.7] - 2021-08-10

* Added support for the following APIs:
    * XblMatchmakingCreateMatchTicketAsync
    * XblMatchmakingDeleteMatchTicketAsync
    * XblMatchmakingGetMatchTicketDetailsAsync
    * XblMatchmakingGetHopperStatisticsAsync
    * XblMultiplayerGetSessionAsync
    * XblMultiplayerGetSessionByHandleAsync
    * XblMultiplayerQuerySessionsAsync
    * XblMultiplayerSetActivityAsync
    * XblMultiplayerClearActivityAsync
* Added support for XAppCapture* APIs

## [0.4.6] - 2021-07-14

* Added missing XblAnonymousUserType property in XblPermissionCheckResult.
* Added support for the following APIs:
    * XblMultiplayerSessionPropertiesSetKeywords
    * XblMultiplayerSessionPropertiesSetJoinRestriction
    * XblMultiplayerSessionPropertiesSetReadRestriction
    * XblMultiplayerSessionPropertiesSetTurnCollection
    * XblMultiplayerWriteSessionByHandleAsync
    * XblMultiplayerSessionSetCustomPropertyJson
    * XblMultiplayerSessionDeleteCustomPropertyJson

## [0.4.5] - 2021-06-18

* Added support for Title Storage APIs.

## [0.4.4] - 2021-05-14

* Null check for XblCleanup callback
* Added support for XLaunchNewGame

## [0.4.3] - 2021-04-14

* Removed check for null user handle in XGameSaveInitializeProvider in order to allow access to the 64 MB local machine storage.

## [0.4.2] - 2021-03-29

* Logging out Exceptions from user callbacks in XTaskQueueDispatch for Development builds as they will cause a crash with no useful information when returning from the PInvoke to the originating native call
* Added support for:
    * XGameUIShowWebAuthenticationAsync
    * XPackageEnumerateFeatures
    * XPackageGetWriteStats
* Added missing value to the XblPresenceFilter enumeration

## [0.4.1] - 2021-03-10

* Added a new sample for Leaderboards.

## [0.4.0] - 2021-02-24

* The package now requires the February 2021 GDK (210200) or later because of breaking changes in the GDK API, specifically the changes in the native XblLeaderboardQuery structure.
* Added the new QueryType property to the XblLeaderboardQuery class, to match the changes in the equivalent native type.
* Added the new XblLeaderboardQueryType enum type.

## [0.3.16] - 2021-02-19

* Fixed XGameUiShowPlayerPickerAsync not returning anything in the array.
* Added support for:
    * XblMultiplayerSendInvitesAsync
    * XblMultiplayerGetActivitiesForSocialGroupAsync
    * XblMultiplayerGetActivitiesWithPropertiesForSocialGroupAsync
    * XblMultiplayerGetActivitiesForUsersAsync
    * XblMultiplayerGetActivitiesWithPropertiesForUsersAsync.

## [0.3.15] - 2021-02-10

* Added support for XStoreShowProductPageUIAsync, XStoreShowAssociatedProductsUIAsync, XStoreIsAvailabilityPurchasable and XStoreAcquireLicenseForDurablesAsync.
* Added public Unity.GameCore.HR class that contains some result error codes.
* Fixed XblMultiplayerActivitySendInvitesAsync and XblMultiplayerActivityGetActivityAsync sending corrupt requests.
* Added XUserAddOptions.AddDefaultUserAllowingUI enum value.

## [0.3.14] - 2021-01-27

* Added support for more Multiplayer APIs and associated types:
    * XblMultiplayerSessionCloseHandle
    * XblMultiplayerSessionSessionProperties
    * XblMultiplayerSessionMembers
    * XblMultiplayerSessionCurrentUser
    * XblMultiplayerSessionSetHostDeviceToken
    * XblMultiplayerSessionSetClosed
    * XblMultiplayerSessionLeave
    * XblMultiplayerSessionCurrentUserSetStatus
    * XblMultiplayerSessionCurrentUserSetSecureDeviceAddressBase64
    * XblFormatSecureDeviceAddress
    * XblMultiplayerSessionSetSessionChangeSubscription.
* Added support for HCSettingsSetTraceLevel, HCSettingsGetTraceLevel and HCTraceSetTraceToDebugger.

## [0.3.13] - 2020-11-25

* Added support for more XblSocial APIs.
* Added support for more XblMultiplayerActivity APIs.

## [0.3.12] - 2020-10-28

* Fixed XblInterop.XblMultiplayerWriteSessionResult not being a Dll import.
* Added support for XblMultiplayerActivitySetActivityAsync and XblMultiplayerActivityDeleteActivityAsync.
* Added support for XblMultiplayerSessionReferenceToUriPath.
* Added support for XblMultiplayerActivityUpdateRecentPlayers and XblMultiplayerActivityFlushRecentPlayersAsync.

## [0.3.11] - 2020-10-14

* Fixed crash in XGameUiShowSendGameInviteAsync.
* Fixed crash due to missing XblMultiplayerManagerMember structure member.
* Added support for the XblMultiplayerSessionCreateHandle API.
* Added multiplayer session write support and session subscription event handlers.
* Added support for XblMultiplayerWriteSessionAsync, XblMultiplayerSessionJoin, XblMultiplayerSetSubscriptionsEnabled and XblMultiplayerSessionWriteStatus.
* Added the XblMultiplayerSessionChanged, XblMultiplayerSessionSubscriptionLost and XblMultiplayerConnectionIdChanged events to the XblContextHandle class.

## [0.3.10] - 2020-09-28

* Added missing XblMultiplayerSessionTag public constructor.

## [0.3.9] - 2020-09-18

* BREAKING CHANGE: Fixed marshaling issues with XGameUiShowTextEntryAsync. The function signature has changed, the requestingUser parameter has been removed.
* Added support for XBL multiplayer search handle APIs and related types.
* Improvement to XblPrivacyBatchCheckPermissionAsync API internal handling.

## [0.3.8] - 2020-09-08

* Fixed bug in XblPrivacyBatchCheckPermissionAsync managed code. (case 1271648)

## [0.3.7] - 2020-08-12

* Fixed a crash when retrieving user profiles.

## [0.3.6] - 2020-08-12

* Added support for XblStringVerify*, XblProfile* and more XGameUi* APIs.

## [0.3.5] - 2020-06-17

* Added support for the GRDK HTTP Client extension web sockets APIs, HCWebSocket*.

## [0.3.4] - 2020-04-22

* Added PLM handling to the Users sample project.
* Because of the PLM event handler code, the Users sample project now requires Unity 2019.3.13f1 (Game Core Preview R3) or newer, or 2020.1.0b8 (Game Core Preview R6) or newer to work.

## [0.3.3] - 2020-04-22

* Added Users sample project.

## [0.3.2] - 2020-03-11

* Fixed errors when importing the package

## [0.3.1] - 2020-02-28

* Internal structure changes.

## [0.3.0] - 2020-02-25

* Renamed the package to "com.unity.gamecore" as there is no need to split the functionality in multiple packages any more.
* BREAKING CHANGE: renamed the namespace from "Unity.GameCore.Foundation" to "Unity.Gamecore".

## [0.2.0] - 2020-02-19

* Removed all native code libraries, the package now contains script code only.

## [0.1.0] - 2020-02-17

* This is the first release of *Unity Package \ Game Core Foundation*.
