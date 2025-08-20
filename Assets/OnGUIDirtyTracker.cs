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
            dirtinessText.text = $"{e.Percentage:0.0}%";
    }
}

public struct DirtinessChangedEvent
{
    public float Percentage;
}
