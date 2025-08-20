using System.Collections.Generic;
using UnityEngine;

public class DotManager : MonoBehaviour
{
    public static DotManager Instance { get; private set; }

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

    public void RegisterDot(DirtySpot dot)
    {
        if (allDots.Add(dot))
        {
            initialDotCount = Mathf.Max(initialDotCount, allDots.Count);
            TriggerDirtinessChanged();
        }
    }

    public void UnregisterDot(DirtySpot dot)
    {
        if (allDots.Remove(dot))
        {
            TriggerDirtinessChanged();
        }
    }

    public float GetDirtPercentage()
    {
        if (initialDotCount == 0) return 0f;
        int dirtyCount = 0;
        foreach (var dot in allDots)
        {
            if (!dot.IsClean)
                dirtyCount++;
        }
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
}