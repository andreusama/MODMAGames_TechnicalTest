using UnityEngine;

public class HUDController : MonoBehaviour, IEventListener<GameEndEvent>
{
    [Header("UI Prefabs")]
    [SerializeField] private EndGameGUI endGameUIPrefab;

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
        if (endGameUIPrefab != null)
        {
            var ui = Instantiate(endGameUIPrefab, transform);
            ui.Initialize(e.Win);
        }
        else
        {
            Debug.LogWarning("EndGameUIPrefab no asignado en HUDController.");
        }
    }
}
