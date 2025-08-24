using UnityEngine;
using System;
using MoreMountains.Feedbacks;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private EnemyConfig m_Config;

    [Header("Enemy Settings")]
    public float MaxHealth;
    [SerializeField] private float m_DestroyDelay;

    [Header("Clean on Death")]
    public float CleanRadius;
    public LayerMask CleanableLayers = ~0; // By default, all layers

    private float m_CurrentHealth;
    private bool m_IsAlive = true;

    public bool IsAlive => m_IsAlive;

    /// <summary>
    /// Fired when the enemy dies.
    /// </summary>
    public event Action<Enemy> OnDied;

    [SerializeField] MMF_Player m_DeathFeedback;

    private void Awake()
    {
        if (m_Config == null)
        {
            Debug.LogWarning($"{name}: EnemyConfig not assigned.");
        }
        else
        {
            MaxHealth = m_Config.MaxHealth;
            m_DestroyDelay = m_Config.DestroyDelay;
            CleanRadius = m_Config.CleanRadius;
            CleanableLayers = m_Config.CleanableLayers;
        }

        m_CurrentHealth = MaxHealth;
        m_IsAlive = true;
    }

    /// <summary>
    /// Applies damage to the enemy.
    /// </summary>
    /// <param name="amount">Damage amount.</param>
    /// <param name="allyFire">Whether damage is friendly fire.</param>
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
    /// Enemy death logic.
    /// </summary>
    private void Die()
    {
        if (!m_IsAlive) return;
        m_IsAlive = false;

        // Show cleaning circle (temporary visual)
        ShowCleanArea();

        CleanNearbyDirt();

        OnDied?.Invoke(this);

        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(m_DestroyDelay);
        if (m_DeathFeedback != null)
        {
            m_DeathFeedback.PlayFeedbacks();
        }

        Destroy(gameObject, m_DeathFeedback.TotalDuration);
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
        // Instantiate a temporary CircleDrawer
        var go = new GameObject("CleanAreaDrawer");
        go.transform.position = transform.position;
        var drawer = go.AddComponent<CircleDrawer>();
        drawer.Segments = 32;
        drawer.DrawCircle(transform.position, CleanRadius);

        // Destroy the circle after a short time
        Destroy(go, 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, CleanRadius);
    }
}
