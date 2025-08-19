using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class DashCollisionHandler : MonoBehaviour
{
    private DashSkill dashSkill;

    private void Awake()
    {
        // Busca la skill de dash en el SkillManager del jugador
        var skillManager = GetComponent<SkillManager>();
        if (skillManager != null)
            dashSkill = skillManager.GetSkill<DashSkill>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("oN TRIGGER ENTERED!");
        if (dashSkill == null || !dashSkill.IsDashing)
            return;

        var wettable = other.GetComponent<IWettable>();
        if (wettable != null && wettable.Wetness > 0)
        {
            var explodable = other.GetComponent<IExplodable>();
            if (explodable != null && !explodable.HasExploded)
            {
                explodable.Explode();
            }
            wettable.SetWetness(0);
        }
    }
}