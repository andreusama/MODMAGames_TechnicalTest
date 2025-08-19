using UnityEngine;

public class Sponge : WettableObject, IExplodable
{
    [Header("Sponge Explosion")]
    public float MaxDamage = 30f;
    public float ExplosionRadius = 3f;
    public GameObject ExplosionEffect;
    public LayerMask TargetLayers;

    public bool HasExploded { get; private set; } = false;

    public void Explode()
    {
        if (HasExploded)
            return;

        HasExploded = true;

        // Efecto visual opcional
        if (ExplosionEffect != null)
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);

        // Daño proporcional a la humedad (0-100)
        float scaledDamage = MaxDamage * (Wetness / 100f);

        // Dañar a todos los IDamageable en el radio
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(scaledDamage);
            }
        }
    }
}