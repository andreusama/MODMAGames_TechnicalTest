using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class SelectableScaler : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private float m_ScaleValue = 1.1f;
        [SerializeField] private float m_ScaleTime = 0.1f;
        [SerializeField] private Ease m_ScaleEase = Ease.Linear;

        private Tween m_Tween;

        void OnDisable()
        {
            if (m_Tween.IsActive())
                m_Tween.Kill();
            m_Tween = null;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            m_Tween = transform.DOScale(1f, m_ScaleTime).SetEase(m_ScaleEase).SetUpdate(true);
        }

        public void OnSelect(BaseEventData eventData)
        {
            m_Tween = transform.DOScale(m_ScaleValue, m_ScaleTime).SetEase(m_ScaleEase).SetUpdate(true);
        }
    }
}