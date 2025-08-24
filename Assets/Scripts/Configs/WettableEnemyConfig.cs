using UnityEngine;

[CreateAssetMenu(fileName = "WettableEnemyConfig", menuName = "Configs/Enemies/Wettable")]
public class WettableEnemyConfig : ScriptableObject
{
    [Header("Explosion")]
    public float ExplosionRadius = 2f;
    public int DotsToSpawn = 10;
    public float DestroyDelay = 1f;
    [Tooltip("Minimum distance between dirt dots")]
    public float MinDotDistance = 0.3f;

    [Header("Slowdown")]
    [Range(0.1f, 1f)]
    public float MinSpeedPercent = 0.3f;

    [Header("Movement")]
    [Tooltip("NavMeshAgent speed to apply on AI (if present).")]
    public float MoveSpeed = 3.5f;
}