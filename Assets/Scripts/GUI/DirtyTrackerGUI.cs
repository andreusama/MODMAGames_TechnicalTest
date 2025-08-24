using UnityEngine;
using TMPro;
using MoreMountains.Tools;

public class DirtyTrackerGUI : MonoBehaviour, IEventListener<DirtinessChangedEvent>
{
    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI m_DirtinessText;
    [SerializeField] private MMProgressBar m_DirtyBar;

    [Header("Win marker (arrow)")]
    [Tooltip("RectTransform of the bar area used to measure width. If empty, uses m_DirtyBar's RectTransform.")]
    [SerializeField] private RectTransform m_BarArea;
    [Tooltip("Arrow RectTransform that marks the winning threshold. Anchor it to the LEFT edge of the bar area.")]
    [SerializeField] private RectTransform m_WinArrow;
    [Tooltip("Winning threshold in [0..1]. Example: 0.8 means 80% clean to win.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_WinThreshold01 = 0.8f;
    [Tooltip("Extra horizontal offset (pixels) applied to the arrow (optional).")]
    [SerializeField] private float m_ArrowOffsetX = 0f;

    private float m_LastBarWidth = -1f;

    private void OnEnable()
    {
        this.EventStartListening<DirtinessChangedEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<DirtinessChangedEvent>();
    }

    public void Start()
    {
        EnsureBarArea();
        SetWinThreshold01(0.3f);
    }
    public void OnEvent(DirtinessChangedEvent e)
    {
        if (m_DirtinessText != null)
            UpdateText(e.Percentage);

        if (m_DirtyBar)
            m_DirtyBar.UpdateBar01(e.Percentage / 100f);
    }

    void UpdateText(float percentage)
    {
        if (m_DirtinessText != null)
        {
            m_DirtinessText.text = $"{percentage:0.0}%";
        }
    }

    // Reposition the arrow when rect changes (resolution/layout)
    private void OnRectTransformDimensionsChange()
    {
        UpdateWinArrowPosition();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EnsureBarArea();
        UpdateWinArrowPosition();
    }
#endif

    /// <summary>
    /// Positions the win arrow along the bar width, based on m_WinThreshold01.
    /// Requires the arrow to be anchored to the LEFT side of m_BarArea.
    /// </summary>
    private void UpdateWinArrowPosition()
    {
        if (m_WinArrow == null) return;
        EnsureBarArea();
        if (m_BarArea == null) return;

        float width = m_BarArea.rect.width;
        if (Mathf.Approximately(width, 0f)) return;

        // Only update when needed
        if (!Mathf.Approximately(width, m_LastBarWidth))
            m_LastBarWidth = width;

        float clamped = Mathf.Clamp01(m_WinThreshold01);
        float x = clamped * width + m_ArrowOffsetX;

        var pos = m_WinArrow.anchoredPosition;
        pos.x = x;
        m_WinArrow.anchoredPosition = pos;
    }

    private void EnsureBarArea()
    {
        if (m_BarArea == null && m_DirtyBar != null)
        {
            m_BarArea = m_DirtyBar.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Public API to set threshold at runtime (0..1) and update arrow.
    /// </summary>
    public void SetWinThreshold01(float t01)
    {
        m_WinThreshold01 = Mathf.Clamp01(t01);
        UpdateWinArrowPosition();
    }
}

public struct DirtinessChangedEvent
{
    public float Percentage;
}
