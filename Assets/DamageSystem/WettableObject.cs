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
    public float MaxScale = 2f; // Valor por defecto, ajustable en el inspector

    private Vector3 initialScale;


    /// <summary>
    /// Evento que se dispara cada vez que cambia la humedad.
    /// </summary>
    public event Action<int> OnWetnessChanged;

    /// <summary>
    /// Método virtual para que las clases hijas puedan reaccionar al cambio de humedad.
    /// </summary>
    protected virtual void OnWetnessChangedVirtual(int wetness) { }

    protected virtual void Awake()
    {
        if (Transform == null)
            Transform = transform;

        initialScale = Transform.localScale;
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
            // Interpola entre la escala inicial y el valor absoluto MaxScale
            Vector3 targetScale = Vector3.Lerp(initialScale, initialScale + (Vector3.one * MaxScale), t);
            Transform.localScale = targetScale;
        }
    }
}