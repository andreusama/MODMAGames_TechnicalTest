using UnityEngine;
using System.Collections;
using Game.Framework;
using EnGUI; // Ensure this using exists for SceneGroup and SceneLoaderManager

/// <summary>
/// Game cycle event structures
/// </summary>
public struct GameSpawnPlayer { }
public struct GameStartEvent { }
public struct GameEndEvent
{
    public bool Win;
    public GameEndEvent(bool win) { Win = win; }
}

/// <summary>
/// Fired once per second while the match is running.
/// </summary>
public struct GameSecondTickEvent
{
    public int ElapsedSeconds;    // seconds since start
    public int RemainingSeconds;  // seconds left until GameDuration
    public float Progress01;      // 0..1 (elapsed/GameDuration)
}

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public SceneGroup SceneGroupToLoad;

    [Header("Settings")]
    public float IntroDuration = 2f;
    [Tooltip("Game duration in seconds")]
    public float GameDuration = 60f;
    [Tooltip("Clean percentage required to win (0-1)")]
    [Range(0f, 100f)]
    public float MaxDirtPercentage = 30f;

    private bool m_GameEnded = false;
    private float m_ElapsedTime = 0f;

    // Last whole second sent as tick (-1 means none sent yet)
    private int m_LastTickSecond = -1;

    // Spawn gates
    public bool SpawnedPlayer = false;
    public bool SpongeSpawned = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        Time.timeScale = 1f;
        if (EnGUIManager.Instance.ScreenFader != null)
            yield return EnGUIManager.Instance.FadeOut<IFader>(0.1f);

        // 1) Intro / spawning phase
        EventManager.TriggerEvent(new GameSpawnPlayer());

        // Wait until both player and sponge are spawned
        yield return new WaitUntil(() => SpawnedPlayer && SpongeSpawned);

        if (EnGUIManager.Instance.ScreenFader != null)
            yield return EnGUIManager.Instance.FadeIn<IFader>(1f);

        // 2) Game start
        EventManager.TriggerEvent(new GameStartEvent());

        // Optional: emit tick at 0s immediately
        EmitSecondTickIfChanged();

        // 3) Wait for end condition (win/lose)
        yield return StartCoroutine(WaitForEndCondition());

        Time.timeScale = 0f;

        // 4) Game end
        m_GameEnded = true;
        bool win = CheckWinCondition();
        EventManager.TriggerEvent(new GameEndEvent(win));
    }

    private IEnumerator WaitForEndCondition()
    {
        m_ElapsedTime = 0f;
        m_LastTickSecond = -1;

        while (m_ElapsedTime < GameDuration)
        {
            if (CheckLoseCondition())
                yield break;

            m_ElapsedTime += Time.deltaTime;

            // Emit once when the whole elapsed second changes
            EmitSecondTickIfChanged();

            yield return null;
        }
    }

    private void EmitSecondTickIfChanged()
    {
        int elapsedWhole = Mathf.FloorToInt(m_ElapsedTime);
        if (elapsedWhole == m_LastTickSecond) return;

        m_LastTickSecond = elapsedWhole;

        int remaining = Mathf.Max(0, Mathf.CeilToInt(GameDuration - m_ElapsedTime));
        float progress = Mathf.Clamp01(m_ElapsedTime / GameDuration);

        EventManager.TriggerEvent(new GameSecondTickEvent
        {
            ElapsedSeconds = elapsedWhole,
            RemainingSeconds = remaining,
            Progress01 = progress
        });
    }

    // Win logic: after time has passed, was enough cleaning done?
    private bool CheckWinCondition()
    {
        float dirtiness = DirtyDotManager.Instance != null ? DirtyDotManager.Instance.GetDirtPercentage() : 0f;
        return dirtiness <= MaxDirtPercentage;
    }

    // Lose logic: customize if there are more conditions
    private bool CheckLoseCondition()
    {
        // Example: if player died, etc.
        return false;
    }

    // Public methods to force game end from other scripts
    public void ForceWin()
    {
        if (!m_GameEnded)
        {
            StopAllCoroutines();
            m_GameEnded = true;
            EventManager.TriggerEvent(new GameEndEvent(true));
        }
    }

    public void ForceLose()
    {
        if (!m_GameEnded)
        {
            StopAllCoroutines();
            m_GameEnded = true;
            EventManager.TriggerEvent(new GameEndEvent(false));
        }
    }

    public void LoadNextSecene()
    {
        StartCoroutine(ReloadCoroutine());
    }

    public IEnumerator ReloadCoroutine()
    {
        if (EnGUIManager.Instance.ScreenFader != null)
            yield return EnGUIManager.Instance.FadeOut<IFader>(1f);

        EnGUIManager.Instance.RemoveAllContents();
        yield return new WaitUntil(() => EnGUIManager.Instance.IsEmpty);

        SceneLoaderManager.Instance.LoadSceneGroup(SceneGroupToLoad);
    }
}
