using UnityEngine;

public class DirtBalloon : Balloon
{
    [Header("Dirt Balloon")]
    public GameObject DirtySpotPrefab;
    public int DotsToSpawn = 5;

    public override void Explode()
    {
        if (HasExploded) return;
        m_HasExploded = true;

        // Instancia manchas en posiciones aleatorias dentro del radio de explosión
        for (int i = 0; i < DotsToSpawn; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * ExplosionRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0.2f, randomCircle.y);
            Quaternion decalRotation = Quaternion.Euler(90f, 0f, 0f);
            Instantiate(DirtySpotPrefab, spawnPos, decalRotation);
        }

        Destroy(gameObject);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded)
            return;

        // Comprueba si el objeto con el que colisiona está en las capas de suelo
        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            m_HasTouchedGround = true;

            // Detiene el movimiento y "pega" el globo al punto de impacto
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Ajusta la posición al punto de contacto más cercano
            if (collision.contacts.Length > 0)
            {
                transform.position = collision.contacts[0].point;
            }

            Debug.Log("Before exploding");
            m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }

}