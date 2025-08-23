using UnityEngine;

public class DashCollisionHandler : MonoBehaviour
{
    [Header("Dash effects (all applied)")]
    [SerializeField] private DashEffect[] m_DashEffects;

    private DashSkill m_DashSkill;

    private void Awake()
    {
        // If DashHitbox is a child, SkillManager may be in the parent
        var skillManager = GetComponentInParent<SkillManager>();
        if (skillManager != null)
            m_DashSkill = skillManager.GetSkill<DashSkill>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Minimum conditions to apply effects:
        // - m_DashSkill exists
        // - dash is active
        // - effects are configured
        if (m_DashSkill == null || !m_DashSkill.IsDashing)
            return;

        foreach (var effect in m_DashEffects)
        {
            if (effect != null)
                effect.ApplyEffect(other);
        }
    }
}