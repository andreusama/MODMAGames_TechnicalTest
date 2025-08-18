using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class AlphaFader : MonoBehaviour, IFader
    {
        [SerializeField] private CanvasGroup m_Fader;
        [SerializeField] private Image m_Background;

        public bool IsActive()
        {
            return (m_Fader.alpha == 1f) ? true : false;
        }

        /// <summary>
        /// Fade Coroutine
        /// </summary>
        /// <param name="finalAlpha"></param>
        /// <param name="canvasGroup"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected IEnumerator Fade(float finalAlpha, float duration)
        {
            if (duration <= 0f)
            {
                SetFade(finalAlpha);
                yield break;
            }

            float currentTime = 0f;
            float initialAlphaValue = m_Fader.alpha;

            m_Fader.blocksRaycasts = true;

            while (currentTime <= duration)
            {
                m_Fader.alpha = Mathf.Lerp(initialAlphaValue, finalAlpha, currentTime / duration);
                yield return null;

                currentTime += Time.unscaledDeltaTime;
            }

            m_Fader.alpha = finalAlpha;
            m_Fader.blocksRaycasts = false;
        }

        public void SetFade(float alpha)
        {
            m_Fader.alpha = alpha;
        }

        public void SetFade(float value, Color color)
        {
            m_Background.color = color;
            SetFade(value);
        }

        public IEnumerator FadeSceneIn(float duration)
        {
            SetFade(1f);

            yield return StartCoroutine(Fade(0f, duration));
        }

        public IEnumerator FadeSceneOut(float duration)
        {
            SetFade(0f);

            yield return StartCoroutine(Fade(1f, duration));
        }

        public IEnumerator FadeSceneIn(float duration, Color color)
        {
            m_Background.color = color;

            yield return StartCoroutine(FadeSceneIn(duration));
        }

        public IEnumerator FadeSceneOut(float duration, Color color)
        {
            m_Background.color = color;

            yield return StartCoroutine(FadeSceneOut(duration));
        }

        public void StopAllFadeCoroutines()
        {
            StopAllCoroutines();
        }
    }
}
