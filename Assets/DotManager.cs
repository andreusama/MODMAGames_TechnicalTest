using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DotManager : MonoBehaviour
{
    public static DotManager Instance { get; private set; }

    [Header("Pooling")]
    [Tooltip("Pool de dots (BakedSimpleObjectPooler).")]
    public BakedSimpleObjectPooler dotPooler;

    [Tooltip("Prefab para instanciar cuando el pool está lleno.")]
    public GameObject FallbackDotPrefab;

    private readonly HashSet<DirtySpot> allDots = new HashSet<DirtySpot>();
    private int initialDotCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Registro
    public void RegisterDot(DirtySpot dot)
    {
        if (dot == null) return;
        if (allDots.Add(dot))
        {
            initialDotCount = Mathf.Max(initialDotCount, allDots.Count);
            TriggerDirtinessChanged();
        }
    }

    public void UnregisterDot(DirtySpot dot)
    {
        if (dot == null) return;
        if (allDots.Remove(dot))
        {
            TriggerDirtinessChanged();
        }
    }
    #endregion

    #region Métricas
    public float GetDirtPercentage()
    {
        if (initialDotCount == 0) return 0f;
        int dirtyCount = 0;
        foreach (var dot in allDots)
            if (!dot.IsClean) dirtyCount++;
        return (dirtyCount / (float)initialDotCount) * 100f;
    }

    public int GetTotalDots() => initialDotCount;

    public int GetDirtyDots()
    {
        int dirtyCount = 0;
        foreach (var dot in allDots)
            if (!dot.IsClean) dirtyCount++;
        return dirtyCount;
    }

    private void TriggerDirtinessChanged()
    {
        float percent = GetDirtPercentage();
        EventManager.TriggerEvent(new DirtinessChangedEvent { Percentage = percent });
    }
    #endregion

    #region Spawning público
    /// <summary>
    /// Spawnea (o reutiliza) un dot en la posición/rotación dadas. Devuelve la referencia si se consiguió.
    /// Fallback: instancia si el pool está lleno.
    /// </summary>
    public DirtySpot SpawnDotAt(Vector3 position, Quaternion rotation)
    {
        GameObject dotGO = null;

        if (dotPooler != null)
            dotGO = dotPooler.GetPooledGameObject();

        if (dotGO == null)
        {
            if (FallbackDotPrefab == null) return null;
            dotGO = Instantiate(FallbackDotPrefab, position, rotation);
            var created = dotGO.GetComponent<DirtySpot>() ?? dotGO.AddComponent<DirtySpot>();
            created.ResetState();
            return created;
        }

        dotGO.transform.SetPositionAndRotation(position, rotation);
        var spot = dotGO.GetComponent<DirtySpot>() ?? dotGO.AddComponent<DirtySpot>();
        spot.ResetState();
        return spot;
    }

    /// <summary>
    /// Utilidad para generar varios dots en un radio (por ejemplo explosiones).
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