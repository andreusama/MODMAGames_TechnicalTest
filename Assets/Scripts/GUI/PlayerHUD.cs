using EnGUI;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI Prefabs")]
    [SerializeField] private PlayerHealthGUI m_PlayerHealthGUI;
    [SerializeField] private EndGameGUI m_EndGameUI;

    [Header("Skill Slots (ordered to match SkillManager.Skills)")]
    [SerializeField] private SkillSlotGUI[] m_SkillSlots;

    /// <summary>
    /// Initializes the Player HUD elements that depend on the Player instance.
    /// </summary>
    public void Initialize(PlayerHealth playerHealth, SkillManager skillManager)
    {
        m_PlayerHealthGUI.Initialize(playerHealth);

        // Wire skill slots using the player's SkillManager
        if (skillManager != null && m_SkillSlots != null && m_SkillSlots.Length > 0)
        {
            var skills = skillManager.Skills;
            int count = Mathf.Min(m_SkillSlots.Length, skills != null ? skills.Length : 0);
            for (int i = 0; i < count; i++)
            {
                var src = skills[i] as ICooldownSource;
                if (src != null) m_SkillSlots[i].SetSource(src);
            }
        }
    }

    /// <summary>
    /// Hides gameplay HUD widgets (health, skill slots) without disabling this root.
    /// </summary>
    public void HideHUD()
    {
        if (m_PlayerHealthGUI != null)
            m_PlayerHealthGUI.gameObject.SetActive(false);

        if (m_SkillSlots != null)
        {
            for (int i = 0; i < m_SkillSlots.Length; i++)
            {
                if (m_SkillSlots[i] != null)
                    m_SkillSlots[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Shows the end game UI inside this HUD and disables all other HUD children.
    /// </summary>
    public void ShowEndGame(bool win)
    {
        if (m_EndGameUI == null)
        {
            Debug.LogWarning("PlayerHUD: EndGameUIPrefab is not assigned.");
            return;
        }

        // Instantiate EndGameUI first
        var ui = Instantiate(m_EndGameUI, transform);

        // Disable every other child except EndGameUI
        DeactivateAllExcept(ui.gameObject);

        // Push via EnGUI and initialize
        EnGUIManager.Instance.PushContent(ui);
        ui.Initialize(win);
    }

    //TODO: This is super hacky, consider a better approach. Just did it for the test.
    private void DeactivateAllExcept(GameObject exception)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var childGO = transform.GetChild(i).gameObject;
            if (childGO != exception)
                childGO.SetActive(false);
        }
    }
}