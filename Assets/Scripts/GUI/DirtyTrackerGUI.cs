using UnityEngine;
using TMPro;

public class DirtyTrackerGUI : MonoBehaviour, IEventListener<DirtinessChangedEvent>
{
    [SerializeField] private TMPro.TextMeshProUGUI m_DirtinessText;

    private void OnEnable()
    {
        this.EventStartListening<DirtinessChangedEvent>();

    }

    private void OnDisable()
    {
        this.EventStopListening<DirtinessChangedEvent>();

    }

    public void OnEvent(DirtinessChangedEvent e)
    {
        if (m_DirtinessText != null)
            UpdateText(e.Percentage);
    }

    void UpdateText(float percentage)
    {
        if (m_DirtinessText != null)
        {
            m_DirtinessText.text = $"{percentage:0.0}%";
        }
    }
}

public struct DirtinessChangedEvent
{
    public float Percentage;
}
