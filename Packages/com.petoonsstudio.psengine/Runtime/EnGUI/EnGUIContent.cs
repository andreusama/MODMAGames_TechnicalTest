using MoreMountains.Feedbacks;
using PetoonsStudio.PSEngine.Gameplay;
using PetoonsStudio.PSEngine.Input;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    [Serializable]
    public class EnGUIContentCancelEvent : UnityEvent { }

    /// <summary>
    /// Handles interactability of UI Elements
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class EnGUIContent : MonoBehaviour
    {
        public delegate void EnGUIContentDelegate();

        [Header("References")]
        [SerializeField] protected CanvasGroup m_CanvasGroup;
        [SerializeField] protected GameObject m_Selection;

        [Header("Configuration")]
        [SerializeField] protected bool m_PauseOnOpen = false;
        [Tooltip("Should this content propagate its cancelAction to Selectable children")]
        [SerializeField] protected bool m_HandleCancelEvents;
        [SerializeField, ConditionalHide("m_HandleCancelEvents")] protected bool m_ResetPreviousSelectedOnCancel;

        [Header("Feedbacks")]
        [SerializeField] protected MMF_Player m_OpenFeedback;
        [SerializeField] protected MMF_Player m_CloseFeedback;

        [Header("Events")]
        [Space(5), SerializeField] protected EnGUIContentCancelEvent m_OnCancelAction;
        [Space(5), SerializeField] protected UnityEvent m_OnGainControlEvent;
        [SerializeField] protected UnityEvent m_OnLoseControlEvent;
        [SerializeField] protected UnityEvent m_OpenEvent;
        [SerializeField] protected UnityEvent m_CloseEvent;

        protected float m_PreviousTimeScale = 1f;
        protected GameObject m_PreviousSelectedObject;
        protected EventSystem m_CurrentEventSystem;
        protected List<Selectable> m_SelectableContent;
        protected bool m_ResetPreviousSelected;

        public event EnGUIContentDelegate OnInitialize;
        public event EnGUIContentDelegate OnGainControl;
        public event EnGUIContentDelegate OnLoseControl;
        public event EnGUIContentDelegate OnOpenAnimationComplete;
        public event EnGUIContentDelegate OnCloseAnimationComplete;
        public event EnGUIContentDelegate OnDisableContent;

        public CanvasGroup CanvasGroup => m_CanvasGroup;
        public bool HasControl { get; protected set; }

        public EventSystem CurrentEventSystem => m_CurrentEventSystem;

        protected virtual void Awake()
        {
            if (m_CanvasGroup == null)
                m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Allows to add dynamic content or configure existing content before animating and enabling input.
        /// Called by <see cref="EnGUIManager"/> when pushing a content (hidden or not).
        /// </summary>
        public virtual IEnumerator Initialize()
        {
            OnInitialize?.Invoke();
            yield break;
        }

        /// <summary>
        /// Allows to customize the animation for when the content opened.
        /// Called by <see cref="EnGUIManager"/> when pushing a (non-hidden) content.
        /// </summary>
        /// <param name="lastContentRemoved">The content is being opened because the last content has been removed</param>
        public virtual IEnumerator OpenAnimation(bool lastContentRemoved = false)
        {
            yield return m_OpenFeedback.PlayFeedbacksCoroutine(transform.position);

            OnOpenAnimationComplete?.Invoke();
            m_OpenEvent?.Invoke();

            yield break;
        }

        /// <summary>
        /// Should replicate the same behaviour of <see cref="OpenAnimation"/> but at the end of the animation.
        /// Called by <see cref="EnGUIManager"/> when pushing a (non-hidden) content immediately.
        /// </summary>
        /// <param name="lastContentRemoved">The content is being opened because the last content has been removed</param>
        public virtual IEnumerator OpenImmediately(bool lastContentRemoved = false)
        {
            m_OpenFeedback.SkipToTheEnd();
            yield return new WaitWhile(() => m_OpenFeedback.SkippingToTheEnd);

            OnOpenAnimationComplete?.Invoke();
            m_OpenEvent?.Invoke();
        }

        /// <summary>
        /// Allows to customize the animation for when the content is closed for good.
        /// Called by <see cref="EnGUIManager"/> when removing/closing the content.
        /// </summary>
        /// <param name="newContentPushed">The content is being closed because a new content has been pushed</param>
        public virtual IEnumerator CloseAnimation(bool newContentPushed = false)
        {
            yield return m_CloseFeedback.PlayFeedbacksCoroutine(transform.position);

            OnCloseAnimationComplete?.Invoke();
            m_CloseEvent?.Invoke();

            yield break;
        }

        /// <summary>
        /// Should replicate the same behaviour of <see cref="CloseAnimation"/> but at the end of the animation.
        /// Called by <see cref="EnGUIManager"/> when removing a content immediately.
        /// </summary>
        /// <param name="newContentPushed">The content is being closed because a new content has been pushed</param>
        public virtual IEnumerator CloseImmediately(bool newContentPushed = false)
        {
            m_CloseFeedback.SkipToTheEnd();

            yield return new WaitWhile(() => m_CloseFeedback.SkippingToTheEnd);

            OnCloseAnimationComplete?.Invoke();
            m_CloseEvent?.Invoke();
        }

        /// <summary>
        /// Allows to specify the way in which a content is handled after it is closed. At this point in time, the content is considered as discarded.
        /// The default behaviour is to disable the content's gameObject. However, in some cases you'd like to destroy it instead, or whatever else you desire.
        /// Called by <see cref="EnGUIManager"/> when a content is removed, after all animations and everything else has been done.
        /// </summary>
        public virtual void DisableContent()
        {
            OnDisableContent?.Invoke();
        }

        /// <summary>
        /// Enables the interactability of the content and, if present, restores the previous selection.
        /// Called by <see cref="EnGUIManager"/> when a content is pushed, or when a previous one is restored after removing the last content.
        /// It's called right after OpenAnimation in the former, or RecoverAnimation in the latter.
        /// </summary>
        public virtual void GainControl(EventSystem currentEventSystem = null)
        {
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;

            if (currentEventSystem == null)
                m_CurrentEventSystem = EventSystem.current;
            else
                m_CurrentEventSystem = currentEventSystem;

            if (m_PreviousSelectedObject != null)
                m_CurrentEventSystem.SetSelectedGameObject(m_PreviousSelectedObject);
            else
                m_CurrentEventSystem.SetSelectedGameObject(m_Selection);

            if (m_HandleCancelEvents)
            {
                m_ResetPreviousSelected = false;
                m_SelectableContent = GetComponentsInChildren<Selectable>(includeInactive: true).ToList();

                foreach (var selectable in m_SelectableContent.Select(x => x.gameObject))
                {
                    if (!selectable.TryGetComponent(out SelectableCancel cancel))
                        cancel = selectable.AddComponent<SelectableCancel>();

                    cancel.OnCancelEvent = m_OnCancelAction;
                    cancel.Content = this;
                }
            }

            HasControl = true;

            OnGainControl?.Invoke();
            m_OnGainControlEvent?.Invoke();
        }

        /// <summary>
        /// Disables the interactability of the content, stores the current selection for future reuse, and restores the timeScale prior
        /// to this content's opening.
        /// Called by <see cref="EnGUIManager"/> on a preexisting content when a new one is pushed, or when a content is removed.
        /// </summary>
        public virtual void LoseControl()
        {
            m_PreviousSelectedObject = m_CurrentEventSystem.currentSelectedGameObject;

            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;

            if (m_HandleCancelEvents)
            {
                if (m_SelectableContent == null) m_SelectableContent = GetComponentsInChildren<Selectable>(includeInactive: true).ToList();

                if (m_ResetPreviousSelected)
                    m_PreviousSelectedObject = m_SelectableContent[0].gameObject;

                foreach (var selectable in m_SelectableContent.Select(x => x.gameObject))
                {
                    if (selectable.gameObject != gameObject)
                    {
                        var component = selectable.GetComponent<SelectableCancel>();
                        Destroy(component);
                    }
                }
            }

            if (EnGUIManager.Instance.IsEmpty)
                m_CurrentEventSystem.SetSelectedGameObject(null);

            HasControl = false;

            OnLoseControl?.Invoke();
            m_OnLoseControlEvent?.Invoke();
        }

        protected virtual void OnValidate()
        {
            if (m_CanvasGroup == null)
                m_CanvasGroup = GetComponent<CanvasGroup>();

            if (m_OpenFeedback == null)
            {
                m_OpenFeedback = new GameObject("OpenFeedback", new Type[] { typeof(MMF_Player) }).GetComponent<MMF_Player>();
                m_OpenFeedback.transform.SetParent(this.transform);

                var canvasGroup = new MMF_CanvasGroup();
                canvasGroup.TargetCanvasGroup = m_CanvasGroup;
                canvasGroup.AlphaCurve.MMTweenDefinitionType = MoreMountains.Tools.MMTweenDefinitionTypes.MMTween;
                canvasGroup.Timing = new MMFeedbackTiming();
                canvasGroup.Timing.TimescaleMode = TimescaleModes.Unscaled;

                m_OpenFeedback.AddFeedback(canvasGroup);
            }

            if (m_CloseFeedback == null)
            {
                m_CloseFeedback = new GameObject("CloseFeedback", new Type[] { typeof(MMF_Player) }).GetComponent<MMF_Player>();
                m_CloseFeedback.transform.SetParent(this.transform);

                var canvasGroup = new MMF_CanvasGroup();
                canvasGroup.TargetCanvasGroup = m_CanvasGroup;
                canvasGroup.AlphaCurve.MMTweenDefinitionType = MoreMountains.Tools.MMTweenDefinitionTypes.MMTween;
                canvasGroup.AlphaCurve.MMTweenCurve = MoreMountains.Tools.MMTween.MMTweenCurve.EaseOutCubic;
                canvasGroup.Timing = new MMFeedbackTiming();
                canvasGroup.Timing.TimescaleMode = TimescaleModes.Unscaled;
                canvasGroup.RemapZero = 1f;
                canvasGroup.RemapOne = 0f;

                m_CloseFeedback.AddFeedback(canvasGroup);
            }
        }

        public void SetTimeScale()
        {
            m_PreviousTimeScale = Time.timeScale;
            if (m_PauseOnOpen)
                Time.timeScale = 0f;
        }

        public void RestoreTimeScale()
        {
            Time.timeScale = m_PreviousTimeScale;
        }

        public virtual void OnCancel()
        {
            if (m_ResetPreviousSelectedOnCancel)
                m_ResetPreviousSelected = true;
        }
    }
}