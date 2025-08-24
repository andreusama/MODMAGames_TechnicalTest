using System;
using UnityEngine;
using Game.Framework;

public class PlayerHealth : MonoBehaviour, IDamageable, IEventListener<DashStartEvent>, IEventListener<DashEndEvent>
{
    [Header("Player Health")]
    public float MaxHealth = 100f;

    private float m_CurrentHealth;
    private bool m_IsAlive = true;

    public bool IsAlive => m_IsAlive;

    private bool m_IsInvulnerable = false;

    public Action<float> OnHealthChanged;

    private void Awake()
    {
        m_CurrentHealth = MaxHealth;
        m_IsAlive = true;
    }

    private void OnEnable()
    {
        this.EventStartListening<DashStartEvent>();
        this.EventStartListening<DashEndEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<DashStartEvent>();
        this.EventStopListening<DashEndEvent>();
    }

    public void OnEvent(DashStartEvent evt)
    {
        m_IsInvulnerable = true;
    }

    public void OnEvent(DashEndEvent evt)
    {
        m_IsInvulnerable = false;
    }

    public void TakeDamage(float amount, bool allyFire = false)
    {
        if (allyFire || m_IsInvulnerable)
            return; // Ignorar daño de aliados o durante dash

        if (!m_IsAlive)
            return;

        m_CurrentHealth -= amount;
        Debug.Log("Current health is " + m_CurrentHealth);
        if (m_CurrentHealth <= 0f)
        {
            m_CurrentHealth = 0f;
            Die();
        }

        OnHealthChanged?.Invoke(m_CurrentHealth);
    }

    private void Die()
    {
        if (!m_IsAlive) return;
        m_IsAlive = false;

        // Notify end of the game
        EventManager.TriggerEvent(new GameEndEvent(false));

        // Death logic
        gameObject.SetActive(false);
    }
}