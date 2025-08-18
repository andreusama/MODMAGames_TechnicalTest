using KBCore.Refs;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField, Self]
    public PlayerMotor PlayerMotor;

    [Header("Skills")]
    public Skill[] Skills;

    private InputMap m_InputActions;

    private void Awake()
    {
        m_InputActions = new InputMap();

        foreach (Skill skill in Skills)
        {
            skill.Initialize(PlayerMotor);
            skill.BindInput(m_InputActions);
        }
    }

    private void OnEnable()
    {
        m_InputActions.Enable();
    }

    private void OnDisable()
    {
        m_InputActions.Disable();

        foreach (Skill skill in Skills)
        {
            skill.UnbindInput(m_InputActions);
        }
    }

    public InputMap GetInputActions() => m_InputActions;
}