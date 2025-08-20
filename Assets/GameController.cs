using UnityEngine;
using System.Collections;
using Game.Framework;
using EnGUI; // Aseg�rate de tener este using para SceneGroup y SceneLoaderManager

/// <summary>
/// Estructuras de eventos para el ciclo de juego
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
    [Header("Configuraci�n")]
    public float IntroDuration = 2f;
    [Tooltip("Duraci�n de la partida en segundos")]
    public float GameDuration = 60f;
    [Tooltip("Porcentaje de limpieza necesario para ganar (0-1)")]
    [Range(0f, 1f)]
    public float RequiredCleanPercentage = 0.8f;
    [Tooltip("Prefab del UI de fin de partida")]
    public EndGameGUI EndGameUIPrefab;
    [Tooltip("Grupo de escenas a recargar al reintentar")]
    public SceneGroup SceneGroupToLoad;

    private bool m_GameStarted = false;
    private bool m_GameEnded = false;
    private float m_ElapsedTime = 0f;

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // 1. Introducci�n
        EventManager.TriggerEvent(new GameIntroEvent());
        yield return new WaitForSeconds(IntroDuration);

        // 2. Inicio del juego
        m_GameStarted = true;
        EventManager.TriggerEvent(new GameStartEvent());

        // 3. Esperar a condici�n de fin (win/lose)
        yield return StartCoroutine(WaitForEndCondition());

        // 4. Fin del juego
        m_GameEnded = true;
        bool win = CheckWinCondition();
        EventManager.TriggerEvent(new GameEndEvent(win));
        ShowEndGameUI(win);
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

    // L�gica de victoria: tras 1 minuto, �se ha limpiado suficiente?
    private bool CheckWinCondition()
    {
        float cleaned = DotManager.Instance != null ? DotManager.Instance.GetDirtPercentage() : 0f;
        return cleaned >= RequiredCleanPercentage;
    }

    // L�gica de derrota: puedes personalizarla si hay otras condiciones
    private bool CheckLoseCondition()
    {
        // Ejemplo: si el jugador muere, etc.
        return false;
    }

    private void ShowEndGameUI(bool win)
    {
        if (EndGameUIPrefab != null)
        {
            var ui = Instantiate(EndGameUIPrefab);
            EnGUIManager.Instance.PushContent(ui);
            ui.GetComponent<EndGameGUI>().Retry(win, SceneGroupToLoad);
        }
    }

    // M�todos p�blicos para forzar el fin del juego desde otros scripts
    public void ForceWin()
    {
        if (!m_GameEnded)
        {
            StopAllCoroutines();
            m_GameEnded = true;
            EventManager.TriggerEvent(new GameEndEvent(true));
            ShowEndGameUI(true);
        }
    }

    public void ForceLose()
    {
        if (!m_GameEnded)
        {
            StopAllCoroutines();
            m_GameEnded = true;
            EventManager.TriggerEvent(new GameEndEvent(false));
            ShowEndGameUI(false);
        }
    }

    // M�todo para reintentar la partida usando SceneLoaderManager y SceneGroup
    public void Retry()
    {
        SceneLoaderManager.Instance.LoadSceneGroup(SceneGroupToLoad);
    }
}
