using KBCore.Refs;
using UnityEngine;
using System;

public class WettableObject : MonoBehaviour, IWettable
{
    [Range(0, 100)]
    [SerializeField]
    private int m_Wetness = 0;

    public int Wetness
    {
        get => m_Wetness;
        set => SetWetness(value); // Así todo el flujo de eventos y lógica se mantiene centralizado
    }

    [SerializeField, Self]
    public Transform Transform;

    /// <summary>
    /// Evento que se dispara cada vez que cambia la humedad.
    /// </summary>
    public event Action<int> OnWetnessChanged;

    /// <summary>
    /// Método virtual para que las clases hijas puedan reaccionar al cambio de humedad.
    /// </summary>
    protected virtual void OnWetnessChangedVirtual(int wetness) { }

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
        // Ejemplo: cambiar escala según la humedad
        if (Transform != null)
            Transform.localScale = Vector3.one * (2 + m_Wetness / 100f);
    }
}