using KBCore.Refs;
using UnityEngine;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    [SerializeField, Self]
    public PlayerMotor PlayerMotor;

    [Header("Skills")]
    public Skill[] Skills;

    private void Awake()
    {
        foreach (Skill skill in Skills)
        {
            skill.Initialize(PlayerMotor);
        }
    }
    public T GetSkill<T>() where T : Skill => Skills.OfType<T>().FirstOrDefault();
}