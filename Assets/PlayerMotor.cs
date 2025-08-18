using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float smoothTime = 0.2f;

    public float slideFriction = 0.9f;

    private NavMeshAgent agent;
    private Vector2 inputVector = Vector2.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 smoothVelocity = Vector3.zero;

    private InputMap inputActions;

    public DashSkill dashSkill;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Desactiva la rotación automática del agente
        inputActions = new InputMap();
        dashSkill.inputActions = inputActions;
        Debug.Log("Update rotation is: " + agent.updateRotation);
    }

    private void OnEnable()
    {
        inputActions.Player.Move.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Dash.Enable();
        inputActions.Player.Dash.performed += dashSkill.OnDashPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.Disable();
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Dash.Disable();
        inputActions.Player.Dash.performed -= dashSkill.OnDashPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        inputVector = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        inputVector = Vector2.zero;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 targetDir = new Vector3(-inputVector.x, 0f, -inputVector.y);
        if (targetDir.sqrMagnitude > 1f)
            targetDir.Normalize();

        Vector3 targetVelocity = targetDir * moveSpeed;
        currentVelocity = Vector3.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref smoothVelocity,
            smoothTime
        );

        if (inputVector == Vector2.zero && currentVelocity.sqrMagnitude > 0.001f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, slideFriction * Time.deltaTime);
        }

        // Movimiento en espacio global
        Vector3 move = currentVelocity;

        // Rotación manual hacia la dirección de movimiento
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * agent.angularSpeed // Ajusta la velocidad de rotación aquí
            );
        }

        agent.Move(move * Time.deltaTime);
    }
}
