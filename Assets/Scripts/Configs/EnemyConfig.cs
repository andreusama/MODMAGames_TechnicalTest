using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/Enemies/Enemy")]
public class EnemyConfig : ScriptableObject
{
    [Header("Stats")]
    public float MaxHealth = 100f;
    public float DestroyDelay = 1f;

    [Header("Clean on Death")]
    public float CleanRadius = 2f;
    public LayerMask CleanableLayers = ~0;

    [Header("AI (optional)")]
    [Tooltip("If true and the Enemy has no AI, one will be added at runtime.")]
    public bool AddAIIfMissing = false;

    [Tooltip("Optional combat/AI configuration (used by EnemyAI).")]
    public EnemyBaseConfig AIConfig;
}