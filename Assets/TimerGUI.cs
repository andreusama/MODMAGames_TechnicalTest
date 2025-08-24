using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerGUI : MonoBehaviour,
    IEventListener<GameSecondTickEvent>,
    IEventListener<GameStartEvent>,
    IEventListener<GameEndEvent>
{
    [Header("UI")]
    [Tooltip("TextMeshProUGUI to display the time (optional)")]
    [SerializeField] private TextMeshProUGUI m_TimeText;

    [Header("Format")]
    [Tooltip("Composite format for mm:ss")]
    [SerializeField] private string m_Format = "{0:00}:{1:00}";

    private void OnEnable()
    {
        this.EventStartListening<GameSecondTickEvent>();
        this.EventStartListening<GameStartEvent>();
        this.EventStartListening<GameEndEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<GameSecondTickEvent>();
        this.EventStopListening<GameStartEvent>();
        this.EventStopListening<GameEndEvent>();
    }

    // Fired once per second while the match is running
    public void OnEvent(GameSecondTickEvent e)
    {
        int minutes = e.RemainingSeconds / 60;
        int seconds = e.RemainingSeconds % 60;

        SetTimeText(string.Format(m_Format, minutes, seconds));
    }

    // Optional: reset visuals at game start
    public void OnEvent(GameStartEvent e)
    {
        SetTimeText("--:--");
    }

    // Optional: lock visuals at end
    public void OnEvent(GameEndEvent e)
    {
        SetTimeText("00:00");
    }

    private void SetTimeText(string value)
    {
        if (m_TimeText != null) m_TimeText.text = value;
    }
}
