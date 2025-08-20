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
            Instantiate(DirtySpotPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}