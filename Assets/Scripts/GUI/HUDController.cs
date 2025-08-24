using UnityEngine;
using UnityEngine.AddressableAssets;

public class HUDController : MonoBehaviour, IEventListener<GameEndEvent>
{
    public static HUDController Instance { get; private set; }

    [Header("UI Prefabs")]
    [SerializeField] private GameObject m_PlayerHUDPrefab;
    [SerializeField] private EndGameGUI m_EndGameUIPrefab; // Fallback if PlayerHUD is not used

    private PlayerHUD m_PlayerHUDInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.AddListener<GameEndEvent>(this);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<GameEndEvent>(this);
        if (Instance == this) Instance = null;
    }

    // Called from PlayerSpawner once the player is ready
    public void CreateHUD(PlayerHealth playerHealth)
    {
        if (m_PlayerHUDInstance == null)
        {
            if (m_PlayerHUDPrefab == null)
            {
                Debug.LogWarning("HUDController: PlayerHUD prefab not assigned.");
                return;
            }

            var go = Instantiate(m_PlayerHUDPrefab, transform);
            if (!go.TryGetComponent<PlayerHUD>(out m_PlayerHUDInstance))
            {
                Debug.LogWarning("HUDController: instantiated PlayerHUD prefab has no PlayerHUD component. Please assign a prefab with PlayerHUD.");
                return;
            }
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("HUDController: PlayerHealth is null. Skipping HUD initialization.");
            return;
        }

        m_PlayerHUDInstance.Initialize(playerHealth);
    }

    public void OnEvent(GameEndEvent e)
    {
        // Prefer showing end-game via PlayerHUD if available
        if (m_PlayerHUDInstance != null)
        {
            m_PlayerHUDInstance.ShowEndGame(e.Win);
            return;
        }

        // Fallback: instantiate end-game UI directly
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
