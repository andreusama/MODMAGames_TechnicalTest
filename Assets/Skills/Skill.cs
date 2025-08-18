using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Skill : MonoBehaviour
{
    protected PlayerMotor m_PlayerMotor;
    protected CooldownTimer m_Cooldown;

    public virtual void Initialize(PlayerMotor motor)
    {
        m_PlayerMotor = motor;
    }

    public virtual void BindInput(InputMap actions) { }
    public virtual void UnbindInput(InputMap actions) { }

    // Permite inicializar el cooldown desde la skill hija
    protected void InitCooldown(float cooldown)
    {
        m_Cooldown = new CooldownTimer(cooldown);
    }

    protected void SetCooldown(float cooldown)
    {
        m_Cooldown.SetCooldown(cooldown);
    }

    protected bool IsCooldownReady => m_Cooldown == null || m_Cooldown.IsReady;

    protected void StartCooldown()
    {
        m_Cooldown?.Start();
    }
}