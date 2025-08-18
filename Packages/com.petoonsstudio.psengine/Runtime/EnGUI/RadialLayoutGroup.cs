using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class RadialLayoutGroup : LayoutGroup
    {
        [SerializeField] protected float m_Radius;
        [SerializeField] protected bool m_Clockwise;

        [Header("Child Placement")]
        [Range(0f, 360f), SerializeField] protected float m_MinAngle;
        [Range(0f, 360f), SerializeField] protected float m_MaxAngle = 360f;
        [Range(0f, 360f), SerializeField] protected float m_StartAngle;

        [Header("Circle Arc")]
        [Range(0f, 360f), SerializeField] protected float m_CircleMinAngle;
        [Range(0f, 360f), SerializeField] protected float m_CircleMaxAngle = 360f;
        [SerializeField] protected bool m_AdjustPlacementAngle = false;
        [SerializeField, ConditionalHide(nameof(m_AdjustPlacementAngle), false)]
        protected bool m_IgnorePlacementAngleRestriction = false;

        [Header("Child Rotation")]
        [Range(0f, 360f), SerializeField] protected float m_StartElementAngle;
        [SerializeField] protected bool m_RotateElements;

        [Header("Child Width")]
        [SerializeField] protected bool m_ExpandChildWidth;
        [SerializeField] protected float m_ChildWidthFactor = 1f;
        [Range(0f, 360f), SerializeField] protected float m_MaxWidthFactor = 360f;
        [SerializeField] protected bool m_ChildWidthFromRadius;
        [SerializeField] protected float m_ChildWidthRadiusFactor = 0.01f;

        [Header("Child Height")]
        [SerializeField] protected bool m_ExpandChildHeight;
        [SerializeField] protected float m_ChildHeight = 100f;
        [SerializeField] protected bool m_ChildHeightFromRadius;
        [SerializeField] protected float m_ChildHeightRadiusFactor = 0.0025f;

        #region Properties

        public float Radius
        {
            get
            {
                return m_Radius;
            }
            set
            {
                if (m_Radius != value)
                {
                    m_Radius = value;
                    OnValueChanged();
                }
            }
        }

        public bool Clockwise
        {
            get
            {
                return m_Clockwise;
            }
            set
            {
                if (m_Clockwise != value)
                {
                    m_Clockwise = value;
                    OnValueChanged();
                }
            }
        }

        public float MinAngle
        {
            get
            {
                return m_MinAngle;
            }
            set
            {
                if (m_MinAngle != value)
                {
                    m_MinAngle = value;
                    OnValueChanged();
                }
            }
        }

        public float MaxAngle
        {
            get
            {
                return m_MaxAngle;
            }
            set
            {
                if (m_MaxAngle != value)
                {
                    m_MaxAngle = value;
                    OnValueChanged();
                }
            }
        }

        public float StartAngle
        {
            get
            {
                return m_StartAngle;
            }
            set
            {
                if (m_StartAngle != value)
                {
                    m_StartAngle = value;
                    OnValueChanged();
                }
            }
        }

        public bool ExpandChildWidth
        {
            get
            {
                return m_ExpandChildWidth;
            }
            set
            {
                if (m_ExpandChildWidth != value)
                {
                    m_ExpandChildWidth = value;
                    OnValueChanged();
                }
            }
        }

        public float ChildWidthFactor
        {
            get
            {
                return m_ChildWidthFactor;
            }
            set
            {
                if (m_ChildWidthFactor != value)
                {
                    m_ChildWidthFactor = value;
                    OnValueChanged();
                }
            }
        }

        public bool ChildWidthFromRadius
        {
            get
            {
                return m_ChildWidthFromRadius;
            }
            set
            {
                if (m_ChildWidthFromRadius != value)
                {
                    m_ChildWidthFromRadius = value;
                    OnValueChanged();
                }
            }
        }

        public float ChildWidthRadiusFactor
        {
            get
            {
                return m_ChildWidthRadiusFactor;
            }
            set
            {
                if (m_ChildWidthRadiusFactor != value)
                {
                    m_ChildWidthRadiusFactor = value;
                    OnValueChanged();
                }
            }
        }

        public bool ExpandChildHeight
        {
            get
            {
                return m_ExpandChildHeight;
            }
            set
            {
                if (m_ExpandChildHeight != value)
                {
                    m_ExpandChildHeight = value;
                    OnValueChanged();
                }
            }
        }

        public float ChildHeight
        {
            get
            {
                return m_ChildHeight;
            }
            set
            {
                if (m_ChildHeight != value)
                {
                    m_ChildHeight = value;
                    OnValueChanged();
                }
            }
        }

        public bool ChildHeightFromRadius
        {
            get
            {
                return m_ChildHeightFromRadius;
            }
            set
            {
                if (m_ChildHeightFromRadius != value)
                {
                    m_ChildHeightFromRadius = value;
                    OnValueChanged();
                }
            }
        }

        public float ChildHeightRadiusFactor
        {
            get
            {
                return m_ChildHeightRadiusFactor;
            }
            set
            {
                if (m_ChildHeightRadiusFactor != value)
                {
                    m_ChildHeightRadiusFactor = value;
                    OnValueChanged();
                }
            }
        }

        public RectTransform SelfTransform => rectTransform;

        public void OnValueChanged()
        {
            ApplyRadialLayout();
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            ApplyRadialLayout();
        }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

        public override void CalculateLayoutInputVertical()
        {
            ApplyRadialLayout();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            ApplyRadialLayout();
        }

        public void ApplyRadialLayout()
        {
            var children = FetchActiveChildren(out int activeChildrenCount);

            m_Tracker.Clear();

            if (children.Count == 0)
                return;

            rectTransform.sizeDelta = 2f * m_Radius * Vector2.one;

            float angle = CalculateAnglePerChild(activeChildrenCount);

            float countWidthFactor = angle < m_MaxWidthFactor ? angle : m_MaxWidthFactor;

            var transformProperties = GetDrivenTransformProperties(out bool expandChilds);

            for (int i = 0; i < children.Count; i++)
            {
                m_Tracker.Add(this, children[i], transformProperties);
                LayoutChild(children[i], i, angle, expandChilds, countWidthFactor);
            }
        }

        private float CalculateAnglePerChild(float childCount)
        {
            float defaultMaxAngle;
            if (m_AdjustPlacementAngle)
            {
                defaultMaxAngle = Mathf.Abs(m_CircleMaxAngle - m_CircleMinAngle) / childCount;
            }
            else
            {
                defaultMaxAngle = 360f / childCount; // Slices of the circle.
            }

            float angle = defaultMaxAngle;

            if (!(m_AdjustPlacementAngle && m_IgnorePlacementAngleRestriction))
            {
                angle = Mathf.Clamp(defaultMaxAngle, m_MinAngle, m_MaxAngle);
            }

            return angle;
        }

        private void LayoutChild(RectTransform child, int childIndex, float anglePerChild, bool expandChilds, float countWidthFactor)
        {
            float angle = anglePerChild * childIndex;

            angle = Mathf.Clamp(angle, m_CircleMinAngle, m_CircleMaxAngle);

            if (m_Clockwise)
                angle *= -1f;

            angle += m_StartAngle;

            Vector3 vPos = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            child.localPosition = vPos * m_Radius;

            //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

            float elementAngle = m_StartElementAngle;

            if (m_RotateElements)
                elementAngle += angle;

            child.localEulerAngles = new Vector3(0f, 0f, elementAngle);

            if (expandChilds)
            {
                Vector2 expandSize = child.sizeDelta;
                if (m_ExpandChildWidth)
                {
                    expandSize.x = m_ChildWidthFromRadius ? (m_Radius * m_ChildWidthRadiusFactor) * countWidthFactor * m_ChildWidthFactor : countWidthFactor * m_ChildWidthFactor;
                }
                if (m_ExpandChildHeight)
                {
                    expandSize.y = m_ChildHeightFromRadius ? (m_Radius * m_ChildHeightRadiusFactor) * m_ChildHeight : m_ChildHeight;
                }
                child.sizeDelta = expandSize;
            }
        }

        private List<RectTransform> FetchActiveChildren(out int count)
        {
            List<RectTransform> childList = new();

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = transform.GetChild(i) as RectTransform;
                LayoutElement childLayout = child.GetComponent<LayoutElement>();

                if (child == null || !child.gameObject.activeSelf || (childLayout != null && childLayout.ignoreLayout))
                {
                    continue;
                }

                childList.Add(child);
            }

            count = childList.Count;

            return childList;
        }

        private DrivenTransformProperties GetDrivenTransformProperties(out bool expandChilds)
        {
            expandChilds = m_ExpandChildWidth | m_ExpandChildHeight;
            DrivenTransformProperties drivenTransformProperties = DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot;
            if (m_ExpandChildWidth)
            {
                drivenTransformProperties |= DrivenTransformProperties.SizeDeltaX;
            }
            if (m_ExpandChildHeight)
            {
                drivenTransformProperties |= DrivenTransformProperties.SizeDeltaY;
            }
            if (m_RotateElements)
            {
                drivenTransformProperties |= DrivenTransformProperties.Rotation;
            }
            return drivenTransformProperties;
        }
    }
}
