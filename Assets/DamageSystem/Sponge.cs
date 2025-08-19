using UnityEngine;

public class Sponge : WettableObject, IExplodable
{
    [Header("Sponge Explosion")]
    public float MaxDamage = 30f;
    public float ExplosionRadius = 3f;
    public GameObject ExplosionEffect;
    public LayerMask TargetLayers;

    public bool HasExploded { get; private set; } = false;

    private CircleDrawer m_CircleDrawer;
    private bool m_HasShownRadius = false;

    private void Awake()
    {
        m_CircleDrawer = GetComponentInChildren<CircleDrawer>();
        if (m_CircleDrawer == null)
            Debug.LogWarning("CircleDrawer no encontrado en la esponja.");

    }

    protected override void OnWetnessChangedVirtual(int wetness) 
    {
        if (wetness == 100)
        {
            OnWetnessFull();
        }
        else
        {
            OnWetnessNotFullOrExploded();
        }
    }

    private void OnWetnessFull()
    {
        m_HasShownRadius = true;
        if (m_CircleDrawer != null)
            m_CircleDrawer.DrawCircle(transform.position, ExplosionRadius);
    }

    private void OnWetnessNotFullOrExploded()
    {
        m_HasShownRadius = false;
        if (m_CircleDrawer != null)
            m_CircleDrawer.Hide();
    }

    public void Explode()
    {
        OnWetnessNotFullOrExploded();
        
        // Daño proporcional a la humedad (0-100)
        float scaledDamage = MaxDamage * (Wetness / 100f);

        // Dañar a todos los IDamageable en el radio
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                Debug.Log("Damage applied");
                bool isAllyFire = true;
                damageable.TakeDamage(scaledDamage, isAllyFire);
            }
        }
    }
}