using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public class LinearOffsetTexture : MonoBehaviour
    {
        [SerializeField] private float m_HorizontalSpeed;
        [SerializeField] private float m_VerticalSpeed;
        [SerializeField] private float m_Duration;

        private Tween m_Tween;

        void Start()
        {
            Image image = GetComponent<Image>();
            image.material.mainTextureOffset = new Vector2(0f, 0f);

            if (m_Tween != null)
                m_Tween.Kill();

            m_Tween = image.material.DOOffset(new Vector2(1f * m_HorizontalSpeed, 1f * m_VerticalSpeed), m_Duration).SetEase(Ease.Linear).SetLoops(-1).SetUpdate(true);
        }

        private void OnDisable()
        {
            if (m_Tween != null)
                m_Tween.Kill();
        }
    }
}
