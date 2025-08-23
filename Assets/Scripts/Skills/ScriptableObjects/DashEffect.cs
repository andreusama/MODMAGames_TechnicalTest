using UnityEngine;

/// <summary>
/// Abstract base class for dash effects. Inherit and override ApplyEffect for custom logic.
/// </summary>
public abstract class DashEffect : ScriptableObject
{
    /// <summary>
    /// Applies the dash effect on the collided object.
    /// </summary>
    public abstract void ApplyEffect(Collider other);
}