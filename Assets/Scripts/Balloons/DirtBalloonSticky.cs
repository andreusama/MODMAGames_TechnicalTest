using UnityEngine;

public class DirtBalloonSticky : Balloon
{
    [Header("Dirt Balloon")]
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
        if (m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.linearVelocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
        }

        // In addition to base behavior (reposition with normal)
        base.OnTouchedGroundHook(collision);
    }
}