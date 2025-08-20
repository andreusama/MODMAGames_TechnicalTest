using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable, IEventListener<DashStartEvent>, IEventListener<DashEndEvent>
{
    [Header("Player Health")]
    public float MaxHealth = 100f;

    private float m_CurrentHealth;
    private bool m_IsAlive = true;

    public bool IsAlive => m_IsAlive;

    private bool m_IsInvulnerable = false;

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
        //Now it's only one PlayerMotor, so we can assume it's the current player
        m_IsInvulnerable = true;
    }

    public void OnEvent(DashEndEvent evt)
    {
        //Now it's only one PlayerMotor, so we can assume it's the current player
        m_IsInvulnerable = false;
    }

    public void TakeDamage(float amount, bool allyFire = false)
    {
        if (allyFire || m_IsInvulnerable)
            return; // Ignora el da�o de fuego amigo

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
        // Aqu� puedes a�adir l�gica de muerte, animaciones, eventos, etc.
        gameObject.SetActive(false);
    }
}