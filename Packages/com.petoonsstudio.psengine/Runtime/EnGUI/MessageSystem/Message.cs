using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    /// <summary>
    /// Base class for implementing messages used in the Generic Message System.
    /// </summary>
    /// <typeparam name="TContent">Type of the content of the message.</typeparam>
    [System.Serializable]
    public abstract class Message<T> : IMessage
    {
        [SerializeField] private IMessage.InterruptionType m_Interruption;
        [SerializeField] private int m_Priority;
        [SerializeField] private float m_Duration;
        [SerializeField] private T m_Content;
        [SerializeField] private bool m_WaitIfCutsceneRunning;

        protected Message(IMessage.InterruptionType interruption, int priority, float duration, T content, bool waitCutscene)
        {
            m_Interruption = interruption;
            m_Priority = priority;
            m_Duration = duration;
            m_Content = content;
            m_WaitIfCutsceneRunning = waitCutscene;
        }

        public int Priority => m_Priority;
        public float Duration => m_Duration;
        public IMessage.InterruptionType Interruption => m_Interruption;
        public T Content { get { return m_Content; } set { m_Content = value; } }

        public bool WaitIfCutsceneRunning => m_WaitIfCutsceneRunning;

        public int CompareTo(IMessage other)
        {
            return other.Priority - this.Priority;
        }
    }
}