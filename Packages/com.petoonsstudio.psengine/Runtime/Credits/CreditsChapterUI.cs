using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Credits
{
    [Serializable]
    public class CreditsChapterUI : MonoBehaviour, IPageViewer
    {
        public List<CreditsPageUI> Pages = new List<CreditsPageUI>();

        private CreditsView m_CreditsView;

        [SerializeField, Self] private RectTransform m_RectTransform;

        void Awake()
        {
            m_CreditsView = GetComponentInParent<CreditsView>();
        }

        void OnEnable()
        {
            if (m_CreditsView.ViewMode == CreditsView.CreditsViewMode.StaticPages)
            {
                m_RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                m_RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            }
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public IEnumerator ViewPage()
        {
            gameObject.SetActive(true);

            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].gameObject.SetActive(true);
                yield return new WaitUntil(() => !Pages[i].gameObject.activeInHierarchy);
            }

            gameObject.SetActive(false);
        }
    }
}

