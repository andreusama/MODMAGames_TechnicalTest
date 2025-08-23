using KBCore.Refs;
using UnityEngine;
using System;

public class WettableObject : MonoBehaviour, IWettable
{
    [Range(0, 100)]
    [SerializeField]
    protected int m_Wetness = 0;
    public int Wetness => m_Wetness;

    [SerializeField, Self]
    public Transform Transform;

    [Header("Wettable Scaling")]
    public float MaxScale = 2f; // Default value, adjustable in inspector

    private Vector3 m_InitialScale;


    /// <summary>
    /// Event fired whenever wetness changes.
    /// </summary>
    public event Action<int> OnWetnessChanged;

    /// <summary>
    /// Virtual method to react to wetness changes in derived classes.
    /// </summary>
    protected virtual void OnWetnessChangedVirtual(int wetness) { }

    protected virtual void Awake()
    {
        if (Transform == null)
            Transform = transform;

        m_InitialScale = Transform.localScale;
    }

    public void AddWetness(int amount)
    {
        SetWetness(m_Wetness + amount);
    }

    public void RemoveWetness(int amount)
    {
        SetWetness(m_Wetness - amount);
    }

    public void SetWetness(int value)
    {
        int clamped = Mathf.Clamp(value, 0, 100);
        if (m_Wetness == clamped) return;
        m_Wetness = clamped;
        OnWetnessChanged?.Invoke(m_Wetness);
        OnWetnessChangedVirtual(m_Wetness);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (Transform != null)
        {
            float t = m_Wetness / 100f;
            // Interpolate between initial scale and absolute MaxScale
            Vector3 targetScale = Vector3.Lerp(m_InitialScale, m_InitialScale + (Vector3.one * MaxScale), t);
            Transform.localScale = targetScale;
        }
    }
}