using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBaseConfig", menuName = "Configs/Enemies/Base")]
public class EnemyBaseConfig : ScriptableObject
{
    [Header("Combat")]
    public float Damage = 10f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;
}