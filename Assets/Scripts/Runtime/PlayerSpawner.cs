using UnityEngine;
using Game.Framework;

public class PlayerSpawner : MonoBehaviour, IEventListener<GameStartEvent>
{
    [Header("Configuración")]
    public GameObject PlayerPrefab;
    public Transform SpawnPoint;
    private GameObject m_PlayerInstance;

    private void OnEnable()
    {
        EventManager.AddListener<GameStartEvent>(this);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<GameStartEvent>(this);
    }

    public void OnEvent(GameStartEvent e)
    {
        if (m_PlayerInstance == null)
        {
            m_PlayerInstance = Instantiate(PlayerPrefab, SpawnPoint.position, SpawnPoint.rotation);
            Debug.Log("Player spawned at " + SpawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Player already spawned.");
        }
    }
}

