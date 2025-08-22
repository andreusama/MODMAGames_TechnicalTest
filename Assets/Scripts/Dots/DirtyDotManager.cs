using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DirtyDotManager : MonoBehaviour
{
    public static DirtyDotManager Instance { get; private set; }

    [Header("Pooling")]
    [Tooltip("Pool de dots (BakedSimpleObjectPooler).")]
    public BakedSimpleObjectPooler dotPooler;

    [Tooltip("Prefab para instanciar cuando el pool est� lleno.")]
    public GameObject FallbackDotPrefab;

    private readonly HashSet<DirtyDot> allDots = new HashSet<DirtyDot>();
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
    public void RegisterDot(DirtyDot dot)
    {
        if (dot == null) return;
        if (allDots.Add(dot))
        {
            initialDotCount = Mathf.Max(initialDotCount, allDots.Count);
            TriggerDirtinessChanged();
        }
    }

    public void UnregisterDot(DirtyDot dot)
    {
        if (dot == null) return;
        if (allDots.Remove(dot))
        {
            TriggerDirtinessChanged();
        }
    }
    #endregion

    #region M�tricas
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

    #region Spawning p�blico
    /// <summary>
    /// Spawnea (o reutiliza) un dot en la posici�n/rotaci�n dadas. Devuelve la referencia si se consigui�.
    /// Fallback: instancia si el pool est� lleno.
    /// </summary>
    public DirtyDot SpawnDotAt(Vector3 position, Quaternion rotation)
    {
        GameObject dotGO = null;

        if (dotPooler != null)
            dotGO = dotPooler.GetPooledGameObject();

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