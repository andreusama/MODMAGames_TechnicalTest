using UnityEngine;

public class DashCollisionHandler : MonoBehaviour
{
    [Header("Efectos del dash (se aplican todos)")]
    [SerializeField] private DashEffect[] dashEffects;

    private DashSkill dashSkill;

    private void Awake()
    {
        // Si el DashHitbox es hijo, SkillManager puede estar en el padre
        var skillManager = GetComponentInParent<SkillManager>();
        if (skillManager != null)
            dashSkill = skillManager.GetSkill<DashSkill>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Condiciones mínimas para aplicar efectos:
        // - dashSkill existe
        // - dash está activo
        // - hay efectos configurados
        if (dashSkill == null || !dashSkill.IsDashing)
            return;

        foreach (var effect in dashEffects)
        {
            if (effect != null)
                effect.ApplyEffect(other);
        }
    }
}