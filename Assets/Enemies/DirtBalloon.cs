using UnityEngine;

public class DirtBalloon : Balloon
{
    [Header("Dirt Balloon")]
    public GameObject DirtySpotPrefab; // Mantenido (puede servir como fallback si se desea)
    public int DotsToSpawn = 5;

    public override void Explode()
    {
        if (HasExploded) return;
        m_HasExploded = true;

        // Ahora usa el DotManager (pool + fallback)
        for (int i = 0; i < DotsToSpawn; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * ExplosionRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0.02f, randomCircle.y);
            Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
            DotManager.Instance?.SpawnDotAt(spawnPos, rot);
        }

        Destroy(gameObject);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded)
            return;

        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            m_HasTouchedGround = true;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (collision.contacts.Length > 0)
                transform.position = collision.contacts[0].point;

            m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }
}