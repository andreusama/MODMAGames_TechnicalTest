using KBCore.Refs;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Credits
{
    [Serializable]
    public class CreditsPageUI : MonoBehaviour
    {
        public TextMeshProUGUI Title;
        public List<CreditsSectionUI> Sections = new List<CreditsSectionUI>();

        [SerializeField, Self] private RectTransform m_RectTransform;

        private CreditsView m_CreditsView;
        private RectTransform m_CanvasRect;

        private float m_CurrentOpenTime;

        void Awake()
        {
            m_CreditsView = GetComponentInParent<CreditsView>();
            m_CanvasRect = m_CreditsView.GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            m_CurrentOpenTime = 0f;

            if (m_CreditsView.ViewMode != CreditsView.CreditsViewMode.VerticalScroll)
            {
                m_RectTransform.pivot = new Vector2(0.5f, 0.5f);
                m_RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                m_RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            }
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        void Update()
        {
            if (m_CreditsView.ViewMode == CreditsView.CreditsViewMode.VerticalScroll)
            {
                m_RectTransform.localPosition += Vector3.up * m_CreditsView.ScrollSpeed * Time.deltaTime;

                var bottomPosition = m_RectTransform.localPosition.y - m_RectTransform.sizeDelta.y;
                if (bottomPosition > m_CanvasRect.sizeDelta.y - m_CreditsView.NextPageMargin)
                    gameObject.SetActive(false);
            }
            else
            {
                m_CurrentOpenTime += Time.deltaTime;

                if (m_CurrentOpenTime > m_CreditsView.PageTime)
                    gameObject.SetActive(false);
            }
        }
    }
}
