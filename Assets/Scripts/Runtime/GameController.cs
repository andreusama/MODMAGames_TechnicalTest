using UnityEngine;
using System.Collections;
using Game.Framework;
using EnGUI; // Asegúrate de tener este using para SceneGroup y SceneLoaderManager

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
    [Header("Configuración")]
    public float IntroDuration = 2f;
    [Tooltip("Duración de la partida en segundos")]
    public float GameDuration = 60f;
    [Tooltip("Porcentaje de limpieza necesario para ganar (0-1)")]
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
        // 1. Introducción
        EventManager.TriggerEvent(new GameIntroEvent());
        yield return new WaitForSeconds(IntroDuration);

        // 2. Inicio del juego
        m_GameStarted = true;
        EventManager.TriggerEvent(new GameStartEvent());

        // 3. Esperar a condición de fin (win/lose)
        yield return StartCoroutine(WaitForEndCondition());

        // 4. Fin del juego
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

    // Lógica de victoria: tras 1 minuto, ¿se ha limpiado suficiente?
    private bool CheckWinCondition()
    {
        float dirtiness = DirtyDotManager.Instance != null ? DirtyDotManager.Instance.GetDirtPercentage() : 0f;
        float cleaned = 1f - (dirtiness / 100f); // Convertir a porcentaje de limpieza (0-1)
        return cleaned >= RequiredCleanPercentage;
    }

    // Lógica de derrota: puedes personalizarla si hay otras condiciones
    private bool CheckLoseCondition()
    {
        // Ejemplo: si el jugador muere, etc.
        return false;
    }

    // Métodos públicos para forzar el fin del juego desde otros scripts
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
