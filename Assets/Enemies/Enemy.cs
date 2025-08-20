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

    [Header("Clean on Death")]
    public float CleanRadius = 2f;
    public LayerMask CleanableLayers = ~0; // Por defecto, todas las capas

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

        // Mostrar círculo de limpieza (visual temporal)
        ShowCleanArea();

        CleanNearbyDirt();

        OnDied?.Invoke(this);
        EventManager.TriggerEvent(new EnemyDiedEvent { enemy = this });

        Destroy(gameObject, destroyDelay);
    }

    private void CleanNearbyDirt()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, CleanRadius, CleanableLayers);
        foreach (var hit in hits)
        {
            var cleanable = hit.GetComponent<ICleanable>();
            if (cleanable != null && !cleanable.IsClean)
            {
                cleanable.Clean();
            }
        }
    }
    private void ShowCleanArea()
    {
        // Instancia un CircleDrawer temporal
        var go = new GameObject("CleanAreaDrawer");
        go.transform.position = transform.position;
        var drawer = go.AddComponent<CircleDrawer>();
        drawer.Segments = 32;
        drawer.DrawCircle(transform.position, CleanRadius);

        // Destruye el círculo tras un breve tiempo
        Destroy(go, 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, CleanRadius);
    }
}
