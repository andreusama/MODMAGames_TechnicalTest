using System;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    protected PlayerMotor m_PlayerMotor;
    protected CooldownTimer m_Cooldown;

    // Evento que emite la propia skill cuando entra en un estado activo (apuntar, dash, etc.)
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

    // Llamar en la skill concreta cuando efectivamente comienza (una sola vez por activación)
    protected void RaiseSkillStarted()
    {
        SkillStarted?.Invoke(this);
    }

    // Cancelación genérica (override si la skill mantiene estado)
    public virtual void Cancel() { }
}