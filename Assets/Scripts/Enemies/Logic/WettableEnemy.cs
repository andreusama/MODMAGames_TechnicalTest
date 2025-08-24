using KBCore.Refs;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WettableEnemy : WettableObject, IExplodable
{
    [Header("Config")]
    [SerializeField] private WettableEnemyConfig m_Config;

    [Header("Wettable Enemy Settings")]
    public float ExplosionRadius;
    public int DotsToSpawn;
    public float DestroyDelay;
    [Tooltip("Minimum distance between dirt dots")]
    public float MinDotDistance;

    [Header("Wettable Slowdown")]
    [SerializeField, Self]
    private WettableEnemyAI m_WettableEnemyAI;

    public event Action<WettableEnemy> OnExplode;
    [SerializeField] MMF_Player m_ExplodeFB;

    [Range(0.1f, 1f)]
    public float MinSpeedPercent = 0.3f; // 30% of original speed when fully wet

    public bool HasExploded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        if (m_Config == null)
        {
            Debug.LogWarning($"{name}: WettableEnemyConfig not assigned.");
        }
        else
        {
            ExplosionRadius = m_Config.ExplosionRadius;
            DotsToSpawn = m_Config.DotsToSpawn;
            DestroyDelay = m_Config.DestroyDelay;
            MinDotDistance = m_Config.MinDotDistance;
            MinSpeedPercent = m_Config.MinSpeedPercent;
        }

        if (m_WettableEnemyAI == null)
            m_WettableEnemyAI = GetComponent<WettableEnemyAI>();
    }

    protected override void OnWetnessChangedVirtual(int wetness)
    {
        base.OnWetnessChangedVirtual(wetness);

        if (m_WettableEnemyAI != null)
        {
            float t = wetness / 100f;
            float percent = Mathf.Lerp(1f, MinSpeedPercent, t);
            m_WettableEnemyAI.SetSpeed(percent);
        }

        if (wetness >= 100 && !HasExploded)
            Explode();
    }

    public void Explode()
    {
        if (HasExploded) return;
        HasExploded = true;

        OnExplode?.Invoke(this);

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
            Vector3 spawnPos = DirtyDotSpawner.Instance.CalculateSpawnPositionWithRaycast(pos);
            DirtyDotManager.Instance?.SpawnDotAt(spawnPos, rot);
        }

        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        // Time to show Die animation
        yield return new WaitForSeconds(DestroyDelay);

        if (m_ExplodeFB != null)
        {
            m_ExplodeFB.PlayFeedbacks();
        }

        Destroy(gameObject, m_ExplodeFB.TotalDuration);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.25f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
#endif
}