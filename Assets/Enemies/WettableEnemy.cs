using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

public class WettableEnemy : WettableObject, IExplodable
{
    [Header("Wettable Enemy Settings")]
    public float ExplosionRadius = 2f;
    public int DotsToSpawn = 10;
    public GameObject DirtySpotPrefab;
    public LayerMask GroundLayers = ~0;
    [Tooltip("Distancia m�nima entre manchas de suciedad")]
    public float MinDotDistance = 0.3f;

    [Header("Wettable Slowdown")]
    [SerializeField, Self]
    private EnemyAI enemyAI;

    [Range(0.1f, 1f)]
    public float MinSpeedPercent = 0.3f; // 30% de la velocidad original cuando est� completamente mojado

    public bool HasExploded { get; private set; } = false;

    private float originalSpeed;

    protected override void Awake()
    {
        base.Awake();
        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
            originalSpeed = enemyAI.GetOriginalSpeed();
    }

    protected override void OnWetnessChangedVirtual(int wetness)
    {
        base.OnWetnessChangedVirtual(wetness);

        if (enemyAI != null)
        {
            float t = wetness / 100f;
            float percent = Mathf.Lerp(1f, MinSpeedPercent, t);
            enemyAI.SetSpeed(originalSpeed * percent);
        }

        if (wetness >= 100 && !HasExploded)
            Explode();
    }

    public void Explode()
    {
        if (HasExploded) return;
        HasExploded = true;

        Vector3 areaCenter = transform.position;
        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
        List<Vector3> dotPositions = DotPlacementUtils.PoissonDiskSample(
            areaCenter,
            new Vector3(ExplosionRadius * 2, 0f, ExplosionRadius * 2),
            MinDotDistance,
            DotsToSpawn
        );

        foreach (var pos in dotPositions)
        {
            Vector3 spawnPos = pos + Vector3.up * 2f;
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 5f, GroundLayers))
                spawnPos = hit.point + Vector3.up * 0.02f;

            DotManager.Instance?.SpawnDotAt(spawnPos, rot);
        }

        Destroy(gameObject, 0.1f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.25f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
#endif
}