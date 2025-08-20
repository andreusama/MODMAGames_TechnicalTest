using UnityEngine;

/// <summary>
/// Clase base abstracta para efectos de dash. Hereda y sobreescribe ApplyEffect para lógica personalizada.
/// </summary>
public abstract class DashEffect : ScriptableObject
{
    /// <summary>
    /// Aplica el efecto de dash sobre el objeto colisionado.
    /// </summary>
    public abstract void ApplyEffect(Collider other);
}