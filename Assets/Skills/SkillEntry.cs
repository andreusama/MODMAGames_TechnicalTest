using UnityEngine;
using GameUtils;

[System.Serializable]
public class SkillEntry : ITableEntry
{
    [SerializeField] private string id;
    [SerializeField] private Skill skill;

    public string ID
    {
        get => id;
        set => id = value;
    }

    public Skill Skill => skill;
}