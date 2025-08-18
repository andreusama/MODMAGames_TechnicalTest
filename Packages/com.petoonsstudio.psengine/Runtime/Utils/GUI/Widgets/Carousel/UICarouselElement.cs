using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public class UICarouselElement : Button
    {
        [System.Serializable]
        public struct ElementAnim
        {
            public float Scale;
            public float Duration;
            public Ease EaseType;
        }

        public int ID;
        public ElementAnim SelectAnim;
        public ElementAnim DeselectAnim;

        public delegate void UICarouselElementEvent(UICarouselElement element);
        public event UICarouselElementEvent OnSelectElement;
        public event UICarouselElementEvent OnDeselectElement;

        private Tween m_Tween;

        public RectTransform RectTransform { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            RectTransform = transform as RectTransform;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_Tween.IsActive())
                m_Tween.Kill();
            m_Tween = null;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnSelectElement?.Invoke(this);
            PlaySelectAnim();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            OnDeselectElement?.Invoke(this);
            PlayDeselectAnim();
        }

        public virtual void PlaySelectAnim()
        {
            m_Tween = transform.DOScale(SelectAnim.Scale, SelectAnim.Duration).SetEase(SelectAnim.EaseType);
        }

        public virtual void PlayDeselectAnim()
        {
            m_Tween = transform.DOScale(DeselectAnim.Scale, DeselectAnim.Duration).SetEase(DeselectAnim.EaseType);
        }
    } 
}