using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SmartScrollRect : ScrollRect
    {
        public bool SetupOnStart = true;
        public bool TimeScaleIndependent = false;

        [Header("Animation")]
        public float ScrollSpeed;
        public Ease ScrollEase;

        private RectTransform m_RectTransform;
        private Tween m_Tween;

        protected override void Awake()
        {
            m_RectTransform = transform as RectTransform;
        }

        protected override void Start()
        {
            if (SetupOnStart) Setup();
        }

        protected override void OnDisable()
        {
            if ( m_Tween.IsActive())
                m_Tween.Kill();
            m_Tween = null;
        }

        public void Setup()
        {
            if (m_Tween.IsActive())
                m_Tween.Kill();
            m_Tween = null;

            if (m_RectTransform == null)
            {
                m_RectTransform = transform as RectTransform;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);
            verticalNormalizedPosition = 1f;

            if (!content)
            {
                Debug.LogWarning("[SmartScrollRect] No content transform was assigned. Setup can't run.");
                return;
            }

            for (int i = 0; i < content.childCount; i++)
            {
                var child = content.GetChild(i);
                var item = child.gameObject.AddComponent<SmartScrollRectItem>();
                item.OnSelectItem += OnScrollItemSelected;
            }
        }

        public void ScrollToItem(int childIndex)
        {
            ScrollToItem(content.GetChild(childIndex) as RectTransform);
        }

        private void OnScrollItemSelected(SmartScrollRectItem item)
        {
            ScrollToItem(item.transform as RectTransform);
        }

        private void ScrollToItem(RectTransform item)
        {
            /// Where the element is inside the scroll rect
            Vector3 elementCenterPos = GetWorldPointInWidget(m_RectTransform, GetWidgetWorldPoint(item));

            /// Where the element should be inside the scroll rect
            Vector3 targetPos = GetWorldPointInWidget(m_RectTransform, GetWidgetWorldPoint(viewport));

            Vector3 difference = targetPos - elementCenterPos;

            if (!horizontal)
                difference.x = 0f;

            if (!vertical)
                difference.y = 0f;

            Vector2 normalizedDifference;
            normalizedDifference.x = difference.x / (content.rect.size.x - m_RectTransform.rect.size.x);
            normalizedDifference.y = difference.y / (content.rect.size.y - m_RectTransform.rect.size.y);

            if (float.IsNaN(normalizedDifference.x)) normalizedDifference.x = 0f;
            if (float.IsNaN(normalizedDifference.y)) normalizedDifference.y = 0f;

            Vector2 newNormalizedPosition = normalizedPosition - normalizedDifference;

            if (movementType != MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }

            m_Tween.Complete();
            m_Tween = this.DONormalizedPos(newNormalizedPosition, 1f / ScrollSpeed).SetEase(ScrollEase).SetUpdate(TimeScaleIndependent);
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