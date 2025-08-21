using KBCore.Refs;
using UnityEngine;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    [SerializeField, Self] public PlayerMotor PlayerMotor;
    [Header("Skills")]
    public Skill[] Skills;

    private void Awake()
    {
        foreach (var skill in Skills)
        {
            if (skill == null) continue;
            skill.Initialize(PlayerMotor);
            skill.SkillStarted += OnSkillStarted;
        }
    }

    private void OnDestroy()
    {
        foreach (var skill in Skills)
        {
            if (skill == null) continue;
            skill.SkillStarted -= OnSkillStarted;
        }
    }

    private void OnSkillStarted(Skill started)
    {
        // Cancela todas las demás
        foreach (var s in Skills)
        {
            if (s != null && s != started)
                s.Cancel();
        }
    }

    public T GetSkill<T>() where T : Skill => Skills.OfType<T>().FirstOrDefault();
}