using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DirtyDotManager : MonoBehaviour
{
    public static DirtyDotManager Instance { get; private set; }

    [Header("Pooling")]
    [Tooltip("Dot pool (BakedSimpleObjectPooler).")]
    public BakedSimpleObjectPooler DotPooler;

    [Tooltip("Prefab to instantiate when pool is exhausted.")]
    public GameObject FallbackDotPrefab;

    private readonly HashSet<DirtyDot> m_AllDots = new HashSet<DirtyDot>();
    private int m_InitialDotCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Registration
    public void RegisterDot(DirtyDot dot)
    {
        if (dot == null) return;
        if (m_AllDots.Add(dot))
        {
            m_InitialDotCount = Mathf.Max(m_InitialDotCount, m_AllDots.Count);
            TriggerDirtinessChanged();
        }
    }

    public void UnregisterDot(DirtyDot dot)
    {
        if (dot == null) return;
        if (m_AllDots.Remove(dot))
        {
            TriggerDirtinessChanged();
        }
    }
    #endregion

    #region Metrics
    public float GetDirtPercentage()
    {
        if (m_InitialDotCount == 0) return 0f;
        int dirtyCount = 0;
        foreach (var dot in m_AllDots)
            if (!dot.IsClean) dirtyCount++;
        return (dirtyCount / (float)m_InitialDotCount) * 100f;
    }

    public int GetTotalDots() => m_InitialDotCount;

    public int GetDirtyDots()
    {
        int dirtyCount = 0;
        foreach (var dot in m_AllDots)
            if (!dot.IsClean) dirtyCount++;
        return dirtyCount;
    }

    private void TriggerDirtinessChanged()
    {
        float percent = GetDirtPercentage();
        EventManager.TriggerEvent(new DirtinessChangedEvent { Percentage = percent });
    }
    #endregion

    #region Public Spawning
    /// <summary>
    /// Spawns (or reuses) a dot at the given position/rotation. Returns the reference if succeeded.
    /// Fallback: instantiates if pool is full.
    /// </summary>
    public DirtyDot SpawnDotAt(Vector3 position, Quaternion rotation)
    {
        GameObject dotGO = null;

        if (DotPooler != null)
            dotGO = DotPooler.GetPooledGameObject();

        if (dotGO == null)
        {
            if (FallbackDotPrefab == null) return null;
            dotGO = Instantiate(FallbackDotPrefab, position, rotation);
            var created = dotGO.GetComponent<DirtyDot>() ?? dotGO.AddComponent<DirtyDot>();
            created.ResetState();
            return created;
        }

        dotGO.transform.SetPositionAndRotation(position, rotation);
        var spot = dotGO.GetComponent<DirtyDot>() ?? dotGO.AddComponent<DirtyDot>();
        spot.ResetState();
        return spot;
    }

    /// <summary>
    /// Utility to spawn multiple dots in a radius (e.g., explosions).
    /// </summary>
    public void SpawnRandomDotsInCircle(Vector3 center, float radius, int count, float yOffset = 0.02f)
    {
        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
        for (int i = 0; i < count; i++)
        {
            Vector2 rnd = Random.insideUnitCircle * radius;
            Vector3 pos = center + new Vector3(rnd.x, yOffset, rnd.y);
            SpawnDotAt(pos, rot);
        }
    }
    #endregion
}