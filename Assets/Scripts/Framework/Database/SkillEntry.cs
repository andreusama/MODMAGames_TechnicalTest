using UnityEngine;
using GameUtils;

[System.Serializable]
public class SkillEntry : ITableEntry
{
    [SerializeField] private string m_Id;
    [SerializeField] private Skill m_Skill;

    public string ID
    {
        get => m_Id;
        set => m_Id = value;
    }

    public Skill Skill => m_Skill;
}