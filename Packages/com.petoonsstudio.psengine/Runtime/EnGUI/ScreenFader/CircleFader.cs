using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class CircleFader : MonoBehaviour, IFader
    {
        [SerializeField] private CanvasGroup m_Fader;
        [SerializeField] private Image m_Background;

        public Ease FadeEase = Ease.InQuad;

        /// Parameters used for the CircleMask shader
        private const float OPEN = 1.35f;
        private const float CLOSED = -0.1f;
        private const string RADIUS_PARAM = "_Radius";
        private const string COLOR_PARAM = "_Color";

        private Tween m_Tween;

        void OnDisable()
        {
            if (m_Tween.IsActive())
                m_Tween.Kill();
            m_Tween = null;
        }

        public bool IsActive()
        {
            return (m_Fader.alpha == 1f) ? true : false;
        }

        /// <summary>
        /// Fade Coroutine
        /// </summary>
        /// <param name="finalValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected IEnumerator Fade(float finalValue, float duration)
        {
            m_Fader.alpha = 1f;

            if (duration <= 0f)
            {
                SetFade(finalValue);
                yield break;
            }

            if (finalValue == OPEN)
                m_Background.material.SetFloat(RADIUS_PARAM, CLOSED);
            else
                m_Background.material.SetFloat(RADIUS_PARAM, OPEN);

            m_Tween = DOTween.To(() => m_Background.material.GetFloat(RADIUS_PARAM), x => m_Background.material.SetFloat(RADIUS_PARAM, x), finalValue, duration).SetEase(FadeEase);
            yield return m_Tween.WaitForCompletion();

            m_Fader.alpha = 0f;
        }

        public void SetFade(float value)
        {
            m_Background.material.SetFloat(RADIUS_PARAM, value);
        }

        public void SetFade(float value, Color color)
        {
            m_Background.material.SetColor(COLOR_PARAM, color);
            SetFade(value);
        }

        public IEnumerator FadeSceneIn(float duration)
        {
            SetFade(CLOSED);

            yield return StartCoroutine(Fade(OPEN, duration));
        }

        public IEnumerator FadeSceneOut(float duration)
        {
            SetFade(OPEN);

            yield return StartCoroutine(Fade(CLOSED, duration));
        }

        public IEnumerator FadeSceneIn(float duration, Color color)
        {
            m_Background.material.SetColor(COLOR_PARAM, color);

            yield return StartCoroutine(FadeSceneIn(duration));
        }

        public IEnumerator FadeSceneOut(float duration, Color color)
        {
            m_Background.material.SetColor(COLOR_PARAM, color);

            yield return StartCoroutine(FadeSceneOut(duration));
        }

        public void StopAllFadeCoroutines()
        {
            StopAllCoroutines();
        }
    }
}

