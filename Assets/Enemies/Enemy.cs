using UnityEngine;

public struct EnemyDiedEvent
{
    public Enemy enemy;
}

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    public float MaxHealth = 100f;

    private float m_CurrentHealth;
    private bool m_IsAlive = true;

    public bool IsAlive => m_IsAlive;

    private void Awake()
    {
        m_CurrentHealth = MaxHealth;
        m_IsAlive = true;
    }

    public void TakeDamage(float amount, bool allyFire)
    {
        if (!m_IsAlive)
            return;

        Debug.Log($"Enemy took {amount} damage. Current health: {m_CurrentHealth - amount}");
        m_CurrentHealth -= amount;
        if (m_CurrentHealth <= 0f)
        {
            m_CurrentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        m_IsAlive = false;
        // Aquí puedes añadir lógica de muerte (animación, desactivar, etc.)
        EventManager.TriggerEvent(new EnemyDiedEvent { enemy = this });
        gameObject.SetActive(false);
    }
}
