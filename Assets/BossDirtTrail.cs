using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BossDirtTrail : MonoBehaviour
{
    public float SpawnInterval = 0.5f;
    public float MinDistance = 0.5f;

    private Vector3 lastSpawnPos;
    private float timer = 0f;
    private static readonly Quaternion k_DecalRotation = Quaternion.Euler(90f, 0f, 0f);

    private void Start()
    {
        lastSpawnPos = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= SpawnInterval && Vector3.Distance(transform.position, lastSpawnPos) > MinDistance)
        {
            Vector3 spawnPos = DotSpawner.Instance.CalculateSpawnPositionWithRaycast(transform.position);
                
            DotManager.Instance?.SpawnDotAt(spawnPos, k_DecalRotation);
            lastSpawnPos = transform.position;
            timer = 0f;
        }
    }
}