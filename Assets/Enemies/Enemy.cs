using UnityEngine;
using System;

public struct EnemyDiedEvent
{
    public Enemy enemy;
}

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    public float MaxHealth = 100f;
    [SerializeField] private float destroyDelay = 1f;

    private float m_CurrentHealth;
    private bool m_IsAlive = true;

    public bool IsAlive => m_IsAlive;

    /// <summary>
    /// Se dispara cuando el enemigo muere.
    /// </summary>
    public event Action<Enemy> OnDied;

    private void Awake()
    {
        m_CurrentHealth = MaxHealth;
        m_IsAlive = true;
    }

    /// <summary>
    /// Aplica daño al enemigo.
    /// </summary>
    /// <param name="amount">Cantidad de daño.</param>
    /// <param name="allyFire">Si el daño es fuego amigo.</param>
    public void TakeDamage(float amount, bool allyFire)
    {
        if (!m_IsAlive || amount <= 0f)
            return;

        m_CurrentHealth -= amount;
        Debug.Log($"Enemy took {amount} damage. Current health: {m_CurrentHealth}");

        if (m_CurrentHealth <= 0f)
        {
            m_CurrentHealth = 0f;
            Die();
        }
    }

    /// <summary>
    /// Lógica de muerte del enemigo.
    /// </summary>
    private void Die()
    {
        if (!m_IsAlive) return;
        m_IsAlive = false;

        OnDied?.Invoke(this);
        EventManager.TriggerEvent(new EnemyDiedEvent { enemy = this });

        // Aquí puedes añadir lógica visual, animaciones, etc.
        Destroy(gameObject, destroyDelay);
    }
}
