using UnityEngine;
using System.Collections;

public class WaterBalloon : Balloon
{
    [Header("Water Balloon")]
    public float Damage = 25f;
    public int WetPower = 20;

    public override void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                bool isAllyFire = true;
                damageable.TakeDamage(Damage, isAllyFire);
            }

            var wettable = hit.GetComponent<IWettable>();
            if (wettable != null)
            {
                wettable.AddWetness(WetPower);
            }

            var cleanable = hit.GetComponent<ICleanable>();
            if (cleanable != null && !cleanable.IsClean)
            {
                cleanable.Clean();
            }
        }

        base.Explode();
    }
}