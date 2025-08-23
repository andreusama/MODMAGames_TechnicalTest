using UnityEngine;

[CreateAssetMenu(fileName = "BalloonGunConfig", menuName = "Configs/Skills/BalloonGun")]
public class BalloonGunConfiguration : ScriptableObject
{
    [Header("Water Balloon")]
    public GameObject WaterBalloonPrefab;
    public float ExplosionDelay = 1.5f;
    public float Cooldown = 2f;
    public float ExplosionRadius = 2.5f;
    public LayerMask TargetLayers = ~0;

    [Header("Aiming")]
    public float MinRange = 2f;
    public float MaxRange = 10f;
    public float MaxHeight = 2f;
    [Range(0.1f, 1f)] public float AimingSpeedPercent = 0.4f;

    [Header("Flight")]
    public float FlightTime = 2f;
    public bool UseAnimationCurve = false;
    public AnimationCurve VelocityCurve = AnimationCurve.Linear(0, 1, 1, 1);
}