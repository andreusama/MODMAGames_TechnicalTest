using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;

    private InputMap inputActions;
    private BalloonGunSkill waterBalloonSkill;
    private DashSkill dashSkill;

    private void Awake()
    {
        inputActions = new InputMap();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        waterBalloonSkill = skillManager.GetSkill<BalloonGunSkill>();
        dashSkill = skillManager.GetSkill<DashSkill>();

        inputActions.Player.ThrowBalloon.performed += OnThrowBalloonPerformed;
        inputActions.Player.ThrowBalloon.canceled += OnThrowBalloonCanceled;
        inputActions.Player.Dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Player.ThrowBalloon.performed -= OnThrowBalloonPerformed;
        inputActions.Player.ThrowBalloon.canceled -= OnThrowBalloonCanceled;
        inputActions.Player.Dash.performed -= OnDashPerformed;
    }

    private void OnThrowBalloonPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        waterBalloonSkill?.OnAimingPerformed(input);
    }

    private void OnThrowBalloonCanceled(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        waterBalloonSkill?.OnAimingCanceled(input);
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        dashSkill?.Activate();
    }
}