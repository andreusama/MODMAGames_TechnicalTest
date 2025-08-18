using UnityEngine;

public class CooldownTimer
{
    private float m_CooldownDuration;
    private float m_NextReadyTime;

    public CooldownTimer(float cooldownDuration)
    {
        m_CooldownDuration = cooldownDuration;
        m_NextReadyTime = 0f;
    }

    public bool IsReady => Time.time >= m_NextReadyTime;

    public void Start()
    {
        m_NextReadyTime = Time.time + m_CooldownDuration;
    }

    public void SetCooldown(float cooldownDuration)
    {
        m_CooldownDuration = cooldownDuration;
    }
}