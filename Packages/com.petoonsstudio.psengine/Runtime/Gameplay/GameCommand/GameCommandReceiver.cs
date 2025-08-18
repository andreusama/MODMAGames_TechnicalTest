using CrossSceneReference;
using KBCore.Refs;
using PetoonsStudio.PSEngine.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static PetoonsStudio.PSEngine.Gameplay.GameCommandSender;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class GameCommandActionData : BehaviourPersistentData
    {
        public int TimesTriggered;

        public GameCommandActionData(string guid, int timesTriggered) : base(guid)
        {
            TimesTriggered = timesTriggered;
        }
    }

    // This class need to be subclassed to implement behaviour based on receiving game command 
    // (see class in SwitchMaterial.cs or PlaySound.cs for sample)
    [SelectionBase]
    [RequireComponent(typeof(GuidComponent))]
    public abstract class GameCommandReceiver : MonoBehaviour, IGameCommandReceiver, IPersistentBehaviour<GameCommandActionData>
    {
        [System.Serializable]
        public struct NotificationResolver
        {
            public GameCommandSender Sender;
            public string Method;
        }

        [SerializeField] protected List<NotificationResolver> m_Notifications;

        [Tooltip("Is this interaction only sent once?")]
        [SerializeField] protected bool m_IsOneShot = false;
        [Tooltip("If this (value) > 0, the interaction will only be sent once every (value) seconds.")]
        [SerializeField] protected float m_CoolDown = 0;

        [Header("Persistence")]
        [SerializeField] protected PersistenceParams m_PersistenceParams;

        protected int m_TimesTriggered;
        protected float m_LastExecutionTime = 0f;
        protected Transform m_InteractingActor;
        [SerializeField, Self] protected GuidComponent m_Guid;
        protected bool m_IsRunning;
        private Dictionary<GameCommandSender, SendNotification> m_RuntimeDelegates;

        public event GameCommandDelegate OnCommandStart;
        public event GameCommandDelegate OnCommandEnd;

        public int TimesTriggered { get => m_TimesTriggered; }

        public PersistenceParams PersistenceParams => m_PersistenceParams;

        public virtual bool IsAvailable
        {
            get
            {
                if (gameObject == null)
                    return false;

                if (!gameObject.activeInHierarchy)
                    return false;

                if (m_IsRunning)
                    return false;

                if (!enabled)
                    return false;

                if (m_IsOneShot && TimesTriggered > 0)
                    return false;

                if (m_CoolDown > 0 && (Time.time - m_LastExecutionTime) < m_CoolDown)
                    return false;

                return true;
            }
        }

        public string Guid
        {
            get
            {
                if (PersistenceParams.IsCustomGuid)
                    return PersistenceParams.CustomGuid;
                else
                    return m_Guid.GetGuid().ToString();
            }
        }

        protected virtual void Awake()
        {
            m_RuntimeDelegates = new Dictionary<GameCommandSender, SendNotification>();
        }

        protected virtual void OnValidate()
        {
            this.ValidateRefs();
        }

        protected virtual void OnEnable()
        {
            foreach (var notification in m_Notifications)
            {
                var delegateFunction = (SendNotification)Delegate.CreateDelegate(typeof(SendNotification), this, notification.Method);
                m_RuntimeDelegates.Add(notification.Sender, delegateFunction);

                notification.Sender.OnNotificationSend += delegateFunction;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var notification in m_Notifications)
            {
                var delegateFunction = m_RuntimeDelegates[notification.Sender];
                notification.Sender.OnNotificationSend -= delegateFunction;

                m_RuntimeDelegates.Remove(notification.Sender);
            }
        }

        protected virtual void Start()
        {
            if (m_PersistenceParams.ShouldPersist) LoadBehaviour();
        }

        public virtual void OnStartAction(Transform source)
        {
            if (!IsAvailable)
            {
                return;
            }

            m_IsRunning = true;

            m_InteractingActor = source;
            m_LastExecutionTime = Time.time;

            StartDelegate();

            Execute();
        }

        protected virtual void EndAction()
        {
            m_TimesTriggered++;

            if (m_PersistenceParams.ShouldPersist) SaveBehaviour();

            EndDelegate();

            m_InteractingActor = null;
            m_IsRunning = false;
        }

        protected void StartDelegate()
        {
            OnCommandStart?.Invoke(transform);
        }

        protected void EndDelegate()
        {
            OnCommandEnd?.Invoke(transform);
        }

        // Implement this in subclass to define the actions that handler should do
        public abstract void Execute();

        public virtual void SaveBehaviour()
        {
            if (GameManager.Instance.CurrentGameMode.Progression == null) return;

            GameManager.Instance.CurrentGameMode.Progression.TrySaveBehaviour(new GameCommandActionData(Guid, m_TimesTriggered));
        }
        public virtual void LoadBehaviour()
        {
            if (GameManager.Instance.CurrentGameMode.Progression == null) return;

            if (GameManager.Instance.CurrentGameMode.Progression.TryLoadBehaviour(Guid, out GameCommandActionData data))
            {
                m_TimesTriggered = data.TimesTriggered;
            }
        }
    }

    public delegate void GameCommandDelegate(Transform executor);

    public interface IGameCommandReceiver
    {
        public event GameCommandDelegate OnCommandStart;
        public event GameCommandDelegate OnCommandEnd;

        public abstract bool IsAvailable { get; }
    }

}
