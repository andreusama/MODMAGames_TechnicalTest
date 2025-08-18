using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Timeline;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    /// <summary>
    /// @Author: Àlex Weiland Lottner
    /// 
    /// Implements functionality for receiving messages in a ordered manner
    /// according to a preset priority. Content of messages can be custom.
    /// 
    /// Coroutines are used for showing, hiding and interrupting messages,
    /// in order to customize the visual representation, animation, interaction, etc.
    /// 
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TContent"></typeparam>
    public abstract class GenericMessageManager : MonoBehaviour
    {
        protected PriorityQueue<IMessage> m_PendingMessages;
        protected IMessage m_RunningMessage;

        public virtual bool MessageIsRunning => m_RunningMessage != null;
        private bool m_IsInInterruption;

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            m_PendingMessages = new PriorityQueue<IMessage>();
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        protected virtual void OnDisable()
        {
            m_RunningMessage = null;
            m_PendingMessages.Clear();
        }

        /// <summary>
        /// Push a new message request
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(IMessage newMessage)
        {
            if (newMessage == null) throw new Exception("Provided message was null");

            switch (newMessage.Interruption)
            {
                case IMessage.InterruptionType.All:
                    InterruptMessage(true);
                    break;
                case IMessage.InterruptionType.Current:
                    InterruptMessage(false);
                    break;
            }

            m_PendingMessages.Push(newMessage);

            TryPlayPendingMessage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="all"></param>
        public void InterruptMessage(bool all = false)
        {
            if (MessageIsRunning)
            {
                if (!m_IsInInterruption)
                {
                    StopAllCoroutines();
                    StartCoroutine(InterruptMessageCo());
                }
            }

            if (all)
                m_PendingMessages.Clear();
        }

        /// <summary>
        /// Looks if any pending messages are available and, if so, 
        /// it plays them back.
        /// </summary>
        /// <returns>Whether a message was available is now being played back</returns>
        private void TryPlayPendingMessage()
        {
            if (MessageIsRunning) return;
            if (m_PendingMessages.Empty) return;

            IMessage pendingMessage = m_PendingMessages.Pop();
            if (pendingMessage == null) return;

            StartCoroutine(PlayMessageCo(pendingMessage));
        }

        /// <summary>
        /// Coroutine with the main behaviour for playing back a message.
        /// </summary>
        /// <param name="message">The message we want to play back</param>
        /// <returns></returns>
        private IEnumerator PlayMessageCo(IMessage message)
        {
            m_RunningMessage = message;

            if (message.WaitIfCutsceneRunning && CutsceneController.InstanceExists)
                yield return new WaitUntil(() => !CutsceneController.Instance.CutsceneRunning && m_PendingMessages.Empty);

            yield return ShowCurrentMessage();

            yield return new WaitForSeconds(message.Duration);

            yield return HideCurrentMessage();

            m_RunningMessage = null;

            TryPlayPendingMessage();
        }

        protected IEnumerator InterruptMessageCo()
        {
            m_IsInInterruption = true;

            yield return InterruptCurrentMessage();

            m_IsInInterruption = false;
            m_RunningMessage = null;

            TryPlayPendingMessage();
        }

        protected abstract IEnumerator ShowCurrentMessage();
        protected abstract IEnumerator HideCurrentMessage();
        protected abstract IEnumerator InterruptCurrentMessage();
    }
}