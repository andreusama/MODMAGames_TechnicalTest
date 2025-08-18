#pragma once

#include "PS4SystemEvents.h"

#if defined(GLOBAL_EVENT_QUEUE)
namespace UnityPlugin
{
    class PzazzSystemEventManager
    {
    public:
        PzazzSystemEventManager() : m_EventQueue(NULL) {}
        ~PzazzSystemEventManager() {}

        void Initialize(UnityEventQueue::IEventQueue* eventQueue);
        void Shutdown();

        void HandleEvent(PzazzOnResume& data);
        void HandleEvent(PzazzOnGameLiveStreamingStatusUpdate& data);
        void HandleEvent(PzazzOnSessionInvitation& data);
        void HandleEvent(PzazzOnEntitlementUpdate& data);
        void HandleEvent(PzazzOnGameCustomData& data);
        void HandleEvent(PzazzOnDisplaySafeAreaUpdate& data);
        void HandleEvent(PzazzOnUrlOpen& data);
        void HandleEvent(PzazzOnLaunchApp& data);
        void HandleEvent(PzazzOnLaunchLink& data);
        void HandleEvent(PzazzOnAddcontentInstall& data);
        void HandleEvent(PzazzOnResetVrPosition& data);
        void HandleEvent(PzazzOnJoinEvent& data);
        void HandleEvent(PzazzOnPlaygoLocusUpdate& data);
        void HandleEvent(PzazzOnOpenShareMenu& data);
        void HandleEvent(PzazzOnPlayTogetherHost& data);
        void HandleEvent(PzazzOnSystemEvent& data);

    private:
        UnityEventQueue::IEventQueue* m_EventQueue;
    };
}
#endif
