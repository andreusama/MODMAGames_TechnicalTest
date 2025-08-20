using UnityEngine;

[CreateAssetMenu(fileName = "DashDamage", menuName = "Skills/Dash Effects/Dash Damage", order = 1)]
public class DashDamage : DashEffect
{
    [Header("Da�o infligido por el dash")]
    public float Damage = 50f;

    public override void ApplyEffect(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            Debug.Log("Applying damage");

            damageable.TakeDamage(Damage, true);
        }
    }
}