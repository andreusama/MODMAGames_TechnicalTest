using KBCore.Refs;
using UnityEngine;

public class WettableObject : MonoBehaviour, IWettable
{
    [Range(0, 100)]
    public float Wetness { get; set; } = 0f;

    [SerializeField, Self]
    public Transform Transform;

    public void AddWetness(float amount)
    {
        Wetness = Mathf.Clamp(Wetness + amount, 0f, 100f);
        // Aquí puedes añadir lógica visual o de peso, por ejemplo:
        // Cambiar color, aumentar peso, etc.
        Transform.localScale = Vector3.one * (2 + Wetness / 100f);
    }

    public void RemoveWetness(float amount)
    {
        Wetness = Mathf.Clamp(Wetness - amount, 0f, 100f);
        // Aquí puedes añadir lógica visual o de peso, por ejemplo:
        // Cambiar color, aumentar peso, etc.
        Transform.localScale = Vector3.one * (2 + Wetness / 100f);
    }

    public void SetWetness(float value)
    {
        Wetness = Mathf.Clamp(value, 0f, 100f);
        // Aquí puedes añadir lógica visual o de peso, por ejemplo:
        // Cambiar color, aumentar peso, etc.
        Transform.localScale = Vector3.one * (2 + Wetness / 100f);
    }
}