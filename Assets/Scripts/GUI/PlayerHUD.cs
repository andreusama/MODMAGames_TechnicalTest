using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI Prefabs")]
    [SerializeField] private PlayerHealthGUI m_PlayerHealthGUI;
    [SerializeField] private EndGameGUI m_EndGameUIPrefab;

    /// <summary>
    /// Initializes the Player HUD elements that depend on the Player instance.
    /// </summary>
    public void Initialize(PlayerHealth playerHealth)
    {
        m_PlayerHealthGUI.Initialize(playerHealth);
    }

    /// <summary>
    /// Shows the end game UI inside this HUD.
    /// </summary>
    public void ShowEndGame(bool win)
    {
        if (m_EndGameUIPrefab == null)
        {
            Debug.LogWarning("PlayerHUD: EndGameUIPrefab is not assigned.");
            return;
        }

        var ui = Instantiate(m_EndGameUIPrefab, transform);
        ui.Initialize(win);
    }
}