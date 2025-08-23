using UnityEngine;

public class HUDController : MonoBehaviour, IEventListener<GameEndEvent>
{
    [Header("UI Prefabs")]
    [SerializeField] private EndGameGUI m_EndGameUIPrefab;

    private void OnEnable()
    {
        EventManager.AddListener<GameEndEvent>(this);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<GameEndEvent>(this);
    }

    public void OnEvent(GameEndEvent e)
    {
        if (m_EndGameUIPrefab != null)
        {
            var ui = Instantiate(m_EndGameUIPrefab, transform);
            ui.Initialize(e.Win);
        }
        else
        {
            Debug.LogWarning("EndGameUIPrefab not assigned in HUDController.");
        }
    }
}
