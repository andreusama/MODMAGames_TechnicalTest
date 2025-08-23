using System;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    protected PlayerMotor m_PlayerMotor;
    protected CooldownTimer m_Cooldown;

    // Event fired by the skill when it enters an active state (aiming, dash, etc.)
    public event Action<Skill> SkillStarted;

    public virtual void Initialize(PlayerMotor motor)
    {
        m_PlayerMotor = motor;
    }

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

    // Call in the concrete skill when it actually starts (once per activation)
    protected void RaiseSkillStarted()
    {
        SkillStarted?.Invoke(this);
    }

    // Generic cancel (override if the skill maintains state)
    public virtual void Cancel() { }
}