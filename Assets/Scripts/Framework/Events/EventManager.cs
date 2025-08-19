//#define EVENTROUTER_THROWEXCEPTIONS 
#if EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
/// Events are structs, you can define any kind of events you want. This manager comes with GameEvents, which are 
/// basically just made of a string, but you can work with more complex ones if you want.
/// 
/// To trigger a new event, from anywhere, just call EventManager.TriggerEvent(YOUR_EVENT);
/// For example : EventManager.TriggerEvent(new GameEvent("GameStart")); will broadcast a GameEvent named GameStart to all listeners.
///
/// To start listening to an event from any class, there are 3 things you must do : 
///
/// 1 - tell that your class implements the IEventListener interface for that kind of event.
/// For example: public class GUIManager : Singleton<GUIManager>, IEventListener<GameEvent>
/// You can have more than one of these (one per event type).
///
/// 2 - On Enable and Disable, respectively start and stop listening to the event :
/// void OnEnable()
/// {
/// 	this.EventStartListening<GameEvent>();
/// }
/// void OnDisable()
/// {
/// 	this.EventStopListening<GameEvent>();
/// }
/// 
/// 3 - Implement the IEventListener interface for that event. For example :
/// public void OnEvent(GameEvent gameEvent)
/// {
/// 	if (gameEvent.eventName == "GameOver")
///		{
///			// DO SOMETHING
///		}
/// } 
/// will catch all events of type GameEvent emitted from anywhere in the game, and do something if it's named GameOver
/// </summary>
[ExecuteInEditMode]
public static class EventManager
{
    private static Dictionary<Type, List<IEventListenerBase>> m_SubscribersList;

    static EventManager()
    {
        m_SubscribersList = new Dictionary<Type, List<IEventListenerBase>>();
    }

    /// <summary>
    /// Adds a new subscriber to a certain event.
    /// </summary>
    public static void AddListener<TEvent>(IEventListener<TEvent> listener) where TEvent : struct
    {
        Type eventType = typeof(TEvent);

        if (!m_SubscribersList.ContainsKey(eventType))
            m_SubscribersList[eventType] = new List<IEventListenerBase>();

        if (!SubscriptionExists(eventType, listener))
            m_SubscribersList[eventType].Add(listener);
    }

    /// <summary>
    /// Removes a subscriber from a certain event.
    /// </summary>
    public static void RemoveListener<TEvent>(IEventListener<TEvent> listener) where TEvent : struct
    {
        Type eventType = typeof(TEvent);

        if (!m_SubscribersList.ContainsKey(eventType))
        {
#if EVENTROUTER_THROWEXCEPTIONS
            throw new ArgumentException($"Removing listener \"{listener}\", but the event type \"{eventType}\" isn't registered.");
#else
            return;
#endif
        }

        List<IEventListenerBase> subscriberList = m_SubscribersList[eventType];
        bool listenerFound = false;

        for (int i = 0; i < subscriberList.Count; i++)
        {
            if (subscriberList[i] == listener)
            {
                subscriberList.Remove(subscriberList[i]);
                listenerFound = true;

                if (subscriberList.Count == 0)
                    m_SubscribersList.Remove(eventType);

                return;
            }
        }

#if EVENTROUTER_THROWEXCEPTIONS
        if (!listenerFound)
        {
            throw new ArgumentException($"Removing listener, but the supplied receiver isn't subscribed to event type \"{eventType}\".");
        }
#endif
    }

    /// <summary>
    /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
    /// </summary>
    public static void TriggerEvent<TEvent>(TEvent newEvent) where TEvent : struct
    {
        List<IEventListenerBase> list;
        if (!m_SubscribersList.TryGetValue(typeof(TEvent), out list))
#if EVENTROUTER_REQUIRELISTENER
            throw new ArgumentException($"Attempting to send event of type \"{typeof(TEvent)}\", but no listener for this type has been found.");
#else
            return;
#endif

        foreach (var listener in list.ToList())
        {
            (listener as IEventListener<TEvent>).OnEvent(newEvent);
        }
    }

    /// <summary>
    /// Checks if there are subscribers for a certain type of events
    /// </summary>
    private static bool SubscriptionExists(Type type, IEventListenerBase receiver)
    {
        List<IEventListenerBase> receivers;

        if (!m_SubscribersList.TryGetValue(type, out receivers)) return false;

        for (int i = 0; i < receivers.Count; i++)
        {
            if (receivers[i] == receiver)
            {
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// Static class that allows any class to start or stop listening to events
/// </summary>
public static class EventRegister
{
    public delegate void Delegate<T>(T eventType);

    public static void EventStartListening<TEvent>(this IEventListener<TEvent> caller) where TEvent : struct
    {
        EventManager.AddListener<TEvent>(caller);
    }

    public static void EventStopListening<TEvent>(this IEventListener<TEvent> caller) where TEvent : struct
    {
        EventManager.RemoveListener<TEvent>(caller);
    }
}

/// <summary>
/// Event listener basic interface
/// </summary>
public interface IEventListenerBase { };

/// <summary>
/// A public interface you'll need to implement for each type of event you want to listen to.
/// </summary>
public interface IEventListener<T> : IEventListenerBase
{
    void OnEvent(T eventType);
}