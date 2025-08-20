using KBCore.Refs;
using UnityEngine;

public class WettableEnemy : WettableObject, IExplodable
{
    [Header("Wettable Enemy Settings")]
    public float ExplosionRadius = 2f;
    public float ExplosionDamage = 30f;
    public LayerMask ExplosionLayers = ~0;

    [Header("Wettable Slowdown")]
    [SerializeField, Self]
    private EnemyAI enemyAI;

    [Range(0.1f, 1f)]
    public float MinSpeedPercent = 0.3f; // 30% de la velocidad original cuando está completamente mojado

    public bool HasExploded { get; private set; } = false;

    private float originalSpeed;

    protected override void Awake()
    {
        base.Awake();
        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
            originalSpeed = enemyAI.GetOriginalSpeed();
    }

    protected override void OnWetnessChangedVirtual(int wetness)
    {
        base.OnWetnessChangedVirtual(wetness);

        // Cambia la velocidad del EnemyAI según la humedad
        if (enemyAI != null)
        {
            float t = wetness / 100f;
            float percent = Mathf.Lerp(1f, MinSpeedPercent, t);
            enemyAI.SetSpeed(originalSpeed * percent);
        }

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