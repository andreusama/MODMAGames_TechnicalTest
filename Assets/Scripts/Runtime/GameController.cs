using UnityEngine;
using System.Collections;
using Game.Framework;
using EnGUI; // Ensure this using exists for SceneGroup and SceneLoaderManager

/// <summary>
/// Game cycle event structures
/// </summary>
public struct GameIntroEvent { }
public struct GameStartEvent { }
public struct GameEndEvent
{
    public bool Win;
    public GameEndEvent(bool win) { Win = win; }
}

public class GameController : MonoBehaviour
{
    [Header("Settings")]
    public float IntroDuration = 2f;
    [Tooltip("Game duration in seconds")]
    public float GameDuration = 60f;
    [Tooltip("Clean percentage required to win (0-1)")]
    [Range(0f, 1f)]
    public float RequiredCleanPercentage = 0.8f;

    private bool m_GameStarted = false;
    private bool m_GameEnded = false;
    private float m_ElapsedTime = 0f;

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // 1. Intro
        EventManager.TriggerEvent(new GameIntroEvent());
        yield return new WaitForSeconds(IntroDuration);

        // 2. Game start
        m_GameStarted = true;
        EventManager.TriggerEvent(new GameStartEvent());

        // 3. Wait for end condition (win/lose)
        yield return StartCoroutine(WaitForEndCondition());

        // 4. Game end
        m_GameEnded = true;
        bool win = CheckWinCondition();
        EventManager.TriggerEvent(new GameEndEvent(win));
    }

    private IEnumerator WaitForEndCondition()
    {
        m_ElapsedTime = 0f;
        while (m_ElapsedTime < GameDuration)
        {
            if (CheckLoseCondition())
                yield break;
            m_ElapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Win logic: after time has passed, was enough cleaning done?
    private bool CheckWinCondition()
    {
        float dirtiness = DirtyDotManager.Instance != null ? DirtyDotManager.Instance.GetDirtPercentage() : 0f;
        float cleaned = 1f - (dirtiness / 100f); // Convert to clean percentage (0-1)
        return cleaned >= RequiredCleanPercentage;
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
}
