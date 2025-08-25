using System;
using UnityEngine;

public interface ICooldownSource
{
    bool IsOnCooldown { get; }
    float CooldownProgress01 { get; } // 1 al empezar, 0 cuando termina
}

public abstract class Skill : MonoBehaviour, ICooldownSource
{
    protected PlayerMotor m_PlayerMotor;
    protected CooldownTimer m_Cooldown;

    // Event fired by the skill when it enters an active state (aiming, dash, etc.)
    public event Action<Skill> SkillStarted;

    // Datos para calcular el progreso sin exponer internals
    private float m_LastCooldownDuration = 0f;
    private float m_CooldownStartTime = -1f;

    public virtual void Initialize(PlayerMotor motor)
    {
        m_PlayerMotor = motor;
    }

    protected void InitCooldown(float cooldown)
    {
        m_LastCooldownDuration = cooldown;
        m_Cooldown = new CooldownTimer(cooldown);
    }

    protected void SetCooldown(float cooldown)
    {
        m_LastCooldownDuration = cooldown;
        m_Cooldown.SetCooldown(cooldown);
    }

    protected bool IsCooldownReady => m_Cooldown == null || m_Cooldown.IsReady;

    protected void StartCooldown()
    {
        m_Cooldown?.Start();
        m_CooldownStartTime = Time.time;
    }

    // ICooldownSource
    public bool IsOnCooldown => m_Cooldown != null && !m_Cooldown.IsReady;

    public float CooldownProgress01
    {
        get
        {
            if (m_Cooldown == null || m_Cooldown.IsReady) return 0f;
            if (m_LastCooldownDuration <= 0f || m_CooldownStartTime < 0f) return 1f;
            float elapsed = Time.time - m_CooldownStartTime;
            return Mathf.Clamp01(1f - (elapsed / m_LastCooldownDuration));
        }
    }

    // Call in the concrete skill when it actually starts (once per activation)
    protected void RaiseSkillStarted()
    {
        SkillStarted?.Invoke(this);
    }

    // Generic cancel (override if the skill maintains state)
    public virtual void Cancel() { }
}