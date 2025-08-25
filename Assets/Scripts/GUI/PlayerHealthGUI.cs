using TMPro;
using UnityEngine;

public class PlayerHealthGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_HealthText;
    private PlayerHealth m_PlayerHealth;

    private void OnDisable()
    {
        if (m_PlayerHealth != null)
            m_PlayerHealth.OnHealthChanged -= UpdateHealthDisplay;
    }

    public void Initialize(PlayerHealth playerHealth)
    {
        m_PlayerHealth = playerHealth;

        m_PlayerHealth.OnHealthChanged += UpdateHealthDisplay;

        m_HealthText.text = playerHealth.MaxHealth.ToString();
    }

    private void UpdateHealthDisplay(float currentHealth)
    {
        if (m_HealthText != null)
        {
            m_HealthText.text = currentHealth.ToString();
        }
    }
}
