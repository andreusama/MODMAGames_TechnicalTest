using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// @Author: Àlex Weiland Lottner
    /// 
    /// UI class that inherits from ScrollRect in order to implement
    /// carousel-like behaviour.
    /// 
    /// Carousel elements are instanced using prefabs.
    /// Animations work using DOTween.
    /// </summary>
    public class UICarousel : ScrollRect
    {
        [Tooltip("Prefab of a single element in the carousel.")]
        public GameObject ElementPrefab;
        public RectTransform ScrollMask;

        [Header("Animation")]
        public float ScrollSpeed;
        public Ease ScrollEase;

        public delegate void UICarouselEvent(UICarouselElement element);
        public event UICarouselEvent OnSelectElement;
        public event UICarouselEvent OnDeselectElement;

        protected List<UICarouselElement> m_Elements;
        protected RectTransform m_RectTransform;

        private Tween m_Tween;

        public UICarouselElement SelectedElement { get; protected set; }
        public bool IsEmpty => m_Elements == null || m_Elements.Count < 1;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_RectTransform = transform as RectTransform;

            if (m_Elements == null)
            {
                m_Elements = new List<UICarouselElement>();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_Tween.IsActive())
                m_Tween.Kill();
            m_Tween = null;
        }

        /// <summary>
        /// Instantiates a new element for the carousel.
        /// </summary>
        /// <param name="id">Unique identifier of choice for the element.</param>
        /// <param name="selected">Whether it's the currently selected option or not.</param>
        /// <returns>Returns the instantiated element</returns>
        public virtual UICarouselElement CreateElement(int id, bool selected)
        {
            GameObject elementGO = Instantiate(ElementPrefab, content);
            var element = elementGO.GetComponent<UICarouselElement>();

            element.ID = id;
            element.OnSelectElement += OnElementSelect;
            element.OnDeselectElement += OnElementDeselect;

            m_Elements.Add(element);

            if (selected)
            {
                element.PlaySelectAnim();
                SelectedElement = element;
            }
            else
            {
                element.PlayDeselectAnim();
            }

            return element;
        }

        /// <summary>
        /// Sets the navigation link references between elements.
        /// </summary>
        public virtual void CreateNavigationLinks()
        {
            Navigation prev = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = m_Elements[1].interactable ? m_Elements[1] : null
            };
            m_Elements[0].navigation = prev;

            for (int i = 1; i < m_Elements.Count - 1; i++)
            {
                Navigation current = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = m_Elements[i - 1].interactable ? m_Elements[i - 1] : null,
                    selectOnRight = m_Elements[i + 1].interactable ? m_Elements[i + 1] : null
                };
                m_Elements[i].navigation = current;
            }

            Navigation next = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = m_Elements[m_Elements.Count - 2].interactable ? m_Elements[m_Elements.Count - 2] : null,
            };
            m_Elements[m_Elements.Count - 1].navigation = next;
        }

        public void ScrollToSelectedElement()
        {
            if (SelectedElement != null)
                ScrollToElement(SelectedElement);
        }

        public void SelectElement(int index)
        {
            if (IsEmpty) return;

            var element = m_Elements[Mathf.Clamp(index, 0, m_Elements.Count - 1)];
            SelectElement(element);
        }

        public void SelectElement(UICarouselElement element)
        {
            if (!element) return;
            EventSystem.current.SetSelectedGameObject(element.gameObject);
            OnElementSelect(element);
        }

        public void DeselectCurrentElement()
        {
            if (!SelectedElement) return;

            OnElementDeselect(SelectedElement);
        }

        protected virtual void OnElementSelect(UICarouselElement element)
        {
            SelectedElement = element;
            ScrollToElement(element);
            OnSelectElement?.Invoke(element);
        }
        
        protected virtual void OnElementDeselect(UICarouselElement element)
        {
            OnDeselectElement?.Invoke(element);
        }

        protected virtual void ScrollToElement(UICarouselElement element)
        {
            /// Where the element is inside the scroll rect
            Vector3 elementCenterPos = GetWorldPointInWidget(m_RectTransform, GetWidgetWorldPoint(element.RectTransform));

            /// Where the element should be inside the scroll rect
            Vector3 targetPos = GetWorldPointInWidget(m_RectTransform, GetWidgetWorldPoint(ScrollMask));

            Vector3 difference = targetPos - elementCenterPos;

            if (!horizontal)
                difference.x = 0f;

            if (!vertical)
                difference.y = 0f;

            Vector2 normalizedDifference;
            normalizedDifference.x = difference.x / (content.rect.size.x - m_RectTransform.rect.size.x);
            normalizedDifference.y = difference.y / (content.rect.size.y - m_RectTransform.rect.size.y);

            Vector2 newNormalizedPosition = normalizedPosition - normalizedDifference;

            if (movementType != MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }

            //normalizedPosition = newNormalizedPosition;
            Debug.Log(normalizedPosition);
            m_Tween = DOTween.To(() => normalizedPosition, (x) => normalizedPosition = x, newNormalizedPosition, 1f / ScrollSpeed).SetEase(ScrollEase);
        }

        private Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            /// Pivot position + item size has to be included
            Vector3 pivotOffset = Vector3.zero;
            pivotOffset.x = (0.5f - target.pivot.x) * target.rect.size.x;
            pivotOffset.y = (0.5f - target.pivot.y) * target.rect.size.y;

            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }
        private Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
        {
            return target.InverseTransformPoint(worldPoint);
        }
    }
}
