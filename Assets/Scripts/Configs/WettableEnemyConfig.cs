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

    [Header("AI (optional)")]
    [Tooltip("If true and the WettableEnemy has no AI, one will be added at runtime.")]
    public bool AddAIIfMissing = false;
}