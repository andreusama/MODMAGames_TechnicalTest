using EnGUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DefaultScreenFader : MonoBehaviour, IFader
{
    [SerializeField] private Image m_FadeImage;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    private Coroutine m_CurrentFadeCoroutine;
    private bool m_IsActive = false;

    private void Awake()
    {
        if (m_CanvasGroup == null)
            m_CanvasGroup = GetComponent<CanvasGroup>();
        if (m_FadeImage == null)
            m_FadeImage = GetComponentInChildren<Image>(true);
        m_CanvasGroup.blocksRaycasts = false;
        m_CanvasGroup.interactable = false;
        SetFade(0f, Color.black);
    }

    public IEnumerator FadeSceneIn(float duration, Color color)
    {
        StopAllFadeCoroutines();
        m_CurrentFadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f, duration, color));
        yield return m_CurrentFadeCoroutine;
    }

    public IEnumerator FadeSceneOut(float duration, Color color)
    {
        StopAllFadeCoroutines();
        m_CurrentFadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f, duration, color));
        yield return m_CurrentFadeCoroutine;
    }

    public IEnumerator FadeSceneIn(float duration)
    {
        yield return FadeSceneIn(duration, m_FadeImage.color);
    }

    public IEnumerator FadeSceneOut(float duration)
    {
        yield return FadeSceneOut(duration, m_FadeImage.color);
    }

    public void SetFade(float value)
    {
        SetFade(value, m_FadeImage.color);
    }

    public void SetFade(float value, Color color)
    {
        m_IsActive = value > 0.01f;
        m_CanvasGroup.alpha = Mathf.Clamp01(value);
        m_FadeImage.color = new Color(color.r, color.g, color.b, 1f);
        m_CanvasGroup.blocksRaycasts = m_IsActive;
    }

    public void StopAllFadeCoroutines()
    {
        if (m_CurrentFadeCoroutine != null)
        {
            StopCoroutine(m_CurrentFadeCoroutine);
            m_CurrentFadeCoroutine = null;
        }
    }

    public bool IsActive()
    {
        return m_IsActive;
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, Color color)
    {
        m_IsActive = true;
        m_FadeImage.color = new Color(color.r, color.g, color.b, 1f);
        m_CanvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(from, to, t);
            m_CanvasGroup.alpha = alpha;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        m_CanvasGroup.alpha = to;
        m_IsActive = to > 0.01f;
        m_CanvasGroup.blocksRaycasts = m_IsActive;
        m_CurrentFadeCoroutine = null;
    }
}