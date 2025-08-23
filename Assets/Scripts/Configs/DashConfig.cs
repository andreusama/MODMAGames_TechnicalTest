using UnityEngine;

[CreateAssetMenu(fileName = "DashConfig", menuName = "Configs/Skills/Dash")]
public class DashConfig : ScriptableObject
{
    [Header("Dash")]
    public float DashDistance = 5f;
    public float DashCooldown = 1f;
    public float DashDuration = 0.2f;

    [Header("Movement Curve")]
    public bool UseDashCurve = false;
    public AnimationCurve DashCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Header("Collision/Clamp")]
    [Tooltip("How far to stop from the obstacle when clamping the dash end point.")]
    public float SafeOffset = 1.1f;

    [Tooltip("Layer name used to ignore dash impact clamping.")]
    public string IgnoreLayerName = "IgnoreDashImpact";
}