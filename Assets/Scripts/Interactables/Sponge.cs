using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public class Sponge : WettableObject, IExplodable
{
    [Header("Sponge Explosion")]
    public float MaxDamage = 30f;
    public float MaxExplosionRadius = 3f;
    public MMF_Player ExplosionFB;
    public LayerMask TargetLayers;

    public bool HasExploded { get; private set; } = false;

    private CircleDrawer m_CircleDrawer;
    private bool m_HasShownRadius = false;

    protected override void Awake()
    {
        base.Awake();

        m_CircleDrawer = GetComponentInChildren<CircleDrawer>();
        if (m_CircleDrawer == null)
            Debug.LogWarning("CircleDrawer no encontrado en la esponja.");
    }

    protected override void OnWetnessChangedVirtual(int wetness)
    {
        UpdateExplosionRadiusVisual();
    }

    private void UpdateExplosionRadiusVisual()
    {
        float scaledRadius = MaxExplosionRadius * (Wetness / 100f);
        if (scaledRadius > 0.01f)
        {
            m_HasShownRadius = true;
            if (m_CircleDrawer != null)
                m_CircleDrawer.DrawCircle(transform.position, scaledRadius);
        }
        else
        {
            m_HasShownRadius = false;
            if (m_CircleDrawer != null)
                m_CircleDrawer.Hide();
        }
    }

    public void Explode()
    {
        UpdateExplosionRadiusVisual();

        float scaledDamage = MaxDamage * (Wetness / 100f);
        float scaledRadius = MaxExplosionRadius * (Wetness / 100f);

        if (scaledRadius <= 0.01f)
            return; // No explota si el radio es 0

        Collider[] hits = Physics.OverlapSphere(transform.position, scaledRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                Debug.Log("Damage applied");
                bool isAllyFire = true;
                damageable.TakeDamage(scaledDamage, isAllyFire);
            }

            var cleanable = hit.GetComponent<ICleanable>();
            if (cleanable != null && !cleanable.IsClean)
            {
                cleanable.Clean();
            }

            var wettable = hit.GetComponent<IWettable>();
            if (wettable != null)
            {
                wettable.AddWetness(Wetness);
            }
        }

        ExplosionFB.PlayFeedbacks();
    }

    
}