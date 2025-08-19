using UnityEngine;
using System.Collections;

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

    private bool m_GameStarted = false;
    private bool m_GameEnded = false;

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
        //yield return StartCoroutine(WaitForEndCondition());

        // 4. Fin del juego
        //m_GameEnded = true;
        //bool win = CheckWinCondition();
        //EventManager.TriggerEvent(new GameEndEvent(win));
    }

    private IEnumerator WaitForEndCondition()
    {
        while (!CheckWinCondition() && !CheckLoseCondition())
        {
            yield return null;
        }
    }

    // Personaliza la lógica de victoria
    private bool CheckWinCondition()
    {
        // TODO: Implementa tu lógica de victoria
        return false;
    }

    // Personaliza la lógica de derrota
    private bool CheckLoseCondition()
    {
        // TODO: Implementa tu lógica de derrota
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
