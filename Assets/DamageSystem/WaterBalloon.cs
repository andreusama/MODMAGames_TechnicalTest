using UnityEngine;
using System.Collections;
using KBCore.Refs;

public class WaterBalloon : MonoBehaviour, IExplodable
{
    [Header("Explosion Settings")]
    public float Damage = 25f;
    public float ExplosionRadius = 2.5f;
    public float ExplosionDelay = 1.5f;
    public LayerMask TargetLayers;
    public LayerMask GroundLayers;

    protected bool m_HasExploded = false;
    protected bool m_HasTouchedGround = false;
    protected Coroutine m_ExplosionCoroutine;

    public bool HasExploded => m_HasExploded;

    [SerializeField, Self]
    protected Rigidbody rb;

    public void Throw(Vector3 launchVelocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = launchVelocity;
        }
        // No se inicia la cuenta atrás aquí, solo al tocar el suelo
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded)
            return;

        // Comprueba si el objeto con el que colisiona está en las capas de suelo
        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            m_HasTouchedGround = true;
            m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }

    protected IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        Explode();
    }

    public void Explode()
    {
        if (m_HasExploded) return;
        m_HasExploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(Damage);
            }
        }

        // Aquí puedes añadir efectos visuales/sonoros
        Destroy(gameObject);
    }
}