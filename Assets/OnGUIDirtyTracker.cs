using UnityEngine;
using TMPro;

public class OnGUIDirtyTracker : MonoBehaviour, IEventListener<DirtinessChangedEvent>
{
    [SerializeField] private TMPro.TextMeshProUGUI dirtinessText;

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
        if (dirtinessText != null)
            UpdateText(e.Percentage);
    }

    void UpdateText(float percentage)
    {
        if (dirtinessText != null)
        {
            dirtinessText.text = $"{percentage:0.0}%";
        }
    }
}

public struct DirtinessChangedEvent
{
    public float Percentage;
}
