using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private SkillManager m_SkillManager;

    private InputMap m_InputActions;
    private BalloonGunSkill m_WaterBalloonSkill;
    private DashSkill m_DashSkill;

    private void Awake()
    {
        m_InputActions = new InputMap();
    }

    private void OnEnable()
    {
        m_InputActions.Enable();

        m_WaterBalloonSkill = m_SkillManager.GetSkill<BalloonGunSkill>();
        m_DashSkill = m_SkillManager.GetSkill<DashSkill>();

        m_InputActions.Player.ThrowBalloon.performed += OnThrowBalloonPerformed;
        m_InputActions.Player.ThrowBalloon.canceled += OnThrowBalloonCanceled;
        m_InputActions.Player.Dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        m_InputActions.Disable();

        m_InputActions.Player.ThrowBalloon.performed -= OnThrowBalloonPerformed;
        m_InputActions.Player.ThrowBalloon.canceled -= OnThrowBalloonCanceled;
        m_InputActions.Player.Dash.performed -= OnDashPerformed;
    }

    private void OnThrowBalloonPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        m_WaterBalloonSkill?.OnAimingPerformed(input);
    }

    private void OnThrowBalloonCanceled(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        m_WaterBalloonSkill?.OnAimingCanceled(input);
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        m_DashSkill?.Activate();
    }
}