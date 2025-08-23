using UnityEngine;

[CreateAssetMenu(fileName = "AutoShootConfig", menuName = "Configs/Skills/AutoShoot")]
public class AutoShotConfiguration : ScriptableObject
{
    [Header("Auto Shoot Timing")]
    public float MinInterval = 2f;
    public float MaxInterval = 4f;

    [Tooltip("Time to keep 'aiming' visuals before the shot is fired.")]
    public float AimTime = 0.5f;

    [Header("Ground Filter")]
    public LayerMask ValidGroundLayers = ~0;
}