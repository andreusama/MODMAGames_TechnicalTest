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
}