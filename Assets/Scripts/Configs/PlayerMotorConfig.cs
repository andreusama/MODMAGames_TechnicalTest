using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMotorConfig", menuName = "Configs/Player/Movement")]
public class PlayerMotorConfig : ScriptableObject
{
    [Header("Movement")]
    public float MoveSpeed = 6f;
    public float SmoothTime = 0.2f;
    public float SlideFriction = 0.9f;
}