using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Player Health")]
    public float MaxHealth = 100f;

    private float m_CurrentHealth;
    private bool m_IsAlive = true;

    public bool IsAlive => m_IsAlive;

    private void Awake()
    {
        m_CurrentHealth = MaxHealth;
        m_IsAlive = true;
    }

    public void TakeDamage(float amount, bool allyFire = false)
    {
        if (allyFire)
            return; // Ignora el daño de fuego amigo

        if (!m_IsAlive)
            return;

        m_CurrentHealth -= amount;
        Debug.Log("Current health is " + m_CurrentHealth);
        if (m_CurrentHealth <= 0f)
        {
            m_CurrentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        m_IsAlive = false;
        // Aquí puedes añadir lógica de muerte, animaciones, eventos, etc.
        gameObject.SetActive(false);
    }
}