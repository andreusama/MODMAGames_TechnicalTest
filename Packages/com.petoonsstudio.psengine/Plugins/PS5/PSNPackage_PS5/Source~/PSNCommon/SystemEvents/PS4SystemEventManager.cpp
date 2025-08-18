#if defined(GLOBAL_EVENT_QUEUE)

#include "../SharedInclude/SonyCommonIncludes.h"
#include "../SharedInclude/HandleMsg.h"
#include "../PlayerInterface/UnityEventQueue.h"

#include "PS4SystemEvents.h"
#include "PS4SystemEventManager.h"

namespace UnityPlugin
{
    //PzazzSystemEventManager PzazzSystemEventManager::s_SystemEventManager;

    UnityEventQueue::ClassBasedEventHandler<PzazzOnResume, PzazzSystemEventManager>                             g_OnResumeAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnGameLiveStreamingStatusUpdate, PzazzSystemEventManager>      g_OnGameLiveStreamingStatusUpdate;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnSessionInvitation, PzazzSystemEventManager>                  g_OnSessionInvitationAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnEntitlementUpdate, PzazzSystemEventManager>                  g_OnEntitlementUpdateAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnGameCustomData, PzazzSystemEventManager>                     g_OnGameCustomDataAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnDisplaySafeAreaUpdate, PzazzSystemEventManager>              g_OnDisplaySafeAreaUpdateAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnUrlOpen, PzazzSystemEventManager>                            g_OnUrlOpenAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnLaunchApp, PzazzSystemEventManager>                          g_OnLaunchAppAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnLaunchLink, PzazzSystemEventManager>                         g_OnLaunchLinkAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnAddcontentInstall, PzazzSystemEventManager>                  g_OnAddcontentInstallAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnResetVrPosition, PzazzSystemEventManager>                    g_OnResetVrPositionAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnJoinEvent, PzazzSystemEventManager>                          g_OnJoinEventAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnPlaygoLocusUpdate, PzazzSystemEventManager>                  g_OnPlaygoLocusUpdateAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnOpenShareMenu, PzazzSystemEventManager>                      g_OnOpenShareMenuAdapter;
    UnityEventQueue::ClassBasedEventHandler<PzazzOnPlayTogetherHost, PzazzSystemEventManager>                   g_OnPlayTogetherHostAdapter;

    UnityEventQueue::ClassBasedEventHandler<PzazzOnSystemEvent, PzazzSystemEventManager>                        g_OnSystemEventAdapter;

    void PzazzSystemEventManager::Initialize(UnityEventQueue::IEventQueue* eventQueue)
    {
        m_EventQueue = eventQueue;

        if (m_EventQueue)
        {
            m_EventQueue->AddHandler(g_OnResumeAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnGameLiveStreamingStatusUpdate.SetObject(this));
            m_EventQueue->AddHandler(g_OnSessionInvitationAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnEntitlementUpdateAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnGameCustomDataAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnDisplaySafeAreaUpdateAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnUrlOpenAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnLaunchAppAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnLaunchLinkAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnAddcontentInstallAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnResetVrPositionAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnJoinEventAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnPlaygoLocusUpdateAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnOpenShareMenuAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnPlayTogetherHostAdapter.SetObject(this));
            m_EventQueue->AddHandler(g_OnSystemEventAdapter.SetObject(this));
        }
    }

    void PzazzSystemEventManager::Shutdown()
    {
        if (m_EventQueue)
        {
            m_EventQueue->RemoveHandler(&g_OnResumeAdapter);
            m_EventQueue->RemoveHandler(&g_OnGameLiveStreamingStatusUpdate);
            m_EventQueue->RemoveHandler(&g_OnSessionInvitationAdapter);
            m_EventQueue->RemoveHandler(&g_OnEntitlementUpdateAdapter);
            m_EventQueue->RemoveHandler(&g_OnGameCustomDataAdapter);
            m_EventQueue->RemoveHandler(&g_OnDisplaySafeAreaUpdateAdapter);
            m_EventQueue->RemoveHandler(&g_OnUrlOpenAdapter);
            m_EventQueue->RemoveHandler(&g_OnLaunchAppAdapter);
            m_EventQueue->RemoveHandler(&g_OnLaunchLinkAdapter);
            m_EventQueue->RemoveHandler(&g_OnAddcontentInstallAdapter);
            m_EventQueue->RemoveHandler(&g_OnResetVrPositionAdapter);
            m_EventQueue->RemoveHandler(&g_OnJoinEventAdapter);
            m_EventQueue->RemoveHandler(&g_OnPlaygoLocusUpdateAdapter);
            m_EventQueue->RemoveHandler(&g_OnOpenShareMenuAdapter);
            m_EventQueue->RemoveHandler(&g_OnPlayTogetherHostAdapter);
            m_EventQueue->RemoveHandler(&g_OnSystemEventAdapter);
        }
    }

    // System events...
    void PzazzSystemEventManager::HandleEvent(PzazzOnResume& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnResume\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnGameLiveStreamingStatusUpdate& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnGameLiveStreamingStatusUpdate\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnSessionInvitation& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnSessionInvitation\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnEntitlementUpdate& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnEntitlementUpdate\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnGameCustomData& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnGameCustomData\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnDisplaySafeAreaUpdate& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnDisplaySafeAreaUpdate\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnUrlOpen& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnUrlOpen\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnLaunchApp& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnLaunchApp\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnLaunchLink& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnLaunchLink\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnAddcontentInstall& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnAddcontentInstall\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnResetVrPosition& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnResetVrPosition\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnJoinEvent& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnJoinEvent\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnPlaygoLocusUpdate& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnPlaygoLocusUpdate\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnOpenShareMenu& data)
    {
        //UNITY_TRACE("PzazzSystemEventManager::HandleEvent: PzazzOnOpenShareMenu\n");
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnPlayTogetherHost& data)
    {
    }

    void PzazzSystemEventManager::HandleEvent(PzazzOnSystemEvent& data)
    {
        psn::MsgHandler::NotifySystemEvent(data.params);
    }

    // App events...
}
#endif
