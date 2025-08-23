using UnityEngine;

public class DirtyTrailSpawner : MonoBehaviour
{
    public float SpawnInterval = 0.5f;
    public float MinDistance = 0.5f;

    private Vector3 m_LastSpawnPos;
    private float m_Timer = 0f;
    private static readonly Quaternion k_DecalRotation = Quaternion.Euler(90f, 0f, 0f);

    private void Start()
    {
        m_LastSpawnPos = transform.position;
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= SpawnInterval && Vector3.Distance(transform.position, m_LastSpawnPos) > MinDistance)
        {
            Vector3 spawnPos = DirtyDotSpawner.Instance.CalculateSpawnPositionWithRaycast(transform.position);
                
            DirtyDotManager.Instance?.SpawnDotAt(spawnPos, k_DecalRotation);
            m_LastSpawnPos = transform.position;
            m_Timer = 0f;
        }
    }
}