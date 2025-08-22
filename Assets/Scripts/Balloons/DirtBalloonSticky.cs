using UnityEngine;

public class DirtBalloonSticky : Balloon
{
    [Header("Dirt Balloon")]
    public GameObject DirtySpotPrefab; // fallback si se desea
    public int DotsToSpawn = 5;

    public override void Explode()
    {
        for (int i = 0; i < DotsToSpawn; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * ExplosionRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0.02f, randomCircle.y);
            Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
            DirtyDotManager.Instance?.SpawnDotAt(spawnPos, rot);
        }

        base.Explode();
    }

    protected override void OnTouchedGroundHook(Collision collision)
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Además del comportamiento base (reposiciona con normal)
        base.OnTouchedGroundHook(collision);
    }
}