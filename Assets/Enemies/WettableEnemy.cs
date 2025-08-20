using UnityEngine;

public class WettableEnemy : WettableObject, IExplodable
{
    [Header("Wettable Enemy Settings")]
    public float ExplosionRadius = 2f;
    public float ExplosionDamage = 30f;
    public LayerMask ExplosionLayers = ~0;

    public bool HasExploded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnWetnessChangedVirtual(int wetness)
    {
        base.OnWetnessChangedVirtual(wetness);

        // Explosión al llegar al máximo
        if (wetness >= 100 && !HasExploded)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (HasExploded) return;
        HasExploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, ExplosionLayers);
        foreach (var hit in hits)
        {
            var cleanable = hit.GetComponent<ICleanable>();
            if (cleanable != null && !cleanable.IsClean)
                cleanable.Clean();
        }

        Destroy(gameObject, 0.1f);
    }
}