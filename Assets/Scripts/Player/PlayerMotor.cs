using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerMotorConfig m_Config;

    [Header("Movement Settings")]
    public float MoveSpeed;
    public float SmoothTime;
    public float SlideFriction;

    [SerializeField, Self]
    private NavMeshAgent m_Agent;
    public NavMeshAgent GetAgent()
    {
        return m_Agent;
    }

    private Vector2 m_InputVector = Vector2.zero;

    private Vector3 m_CurrentVelocity = Vector3.zero;
    public Vector3 GetCurrentVelocity() { return m_CurrentVelocity; }

    private Vector3 m_SmoothVelocity = Vector3.zero;

    private InputMap m_InputActions;

    private void Awake()
    {
        if (m_Config == null)
        {
            Debug.LogWarning($"{name}: PlayerMotorConfig no asignado. Deshabilitando PlayerMotor para evitar interacción del jugador.");
            enabled = false;
            return;
        }

        MoveSpeed = m_Config.MoveSpeed;
        SmoothTime = m_Config.SmoothTime;
        SlideFriction = m_Config.SlideFriction;

        m_Agent.updateRotation = false;
        m_InputActions = new InputMap();
    }

    private void OnEnable()
    {
        if (!enabled) return;

        m_InputActions.Player.Move.Enable();
        m_InputActions.Player.Move.performed += OnMovePerformed;
        m_InputActions.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        m_InputActions.Player.Move.Disable();
        m_InputActions.Player.Move.performed -= OnMovePerformed;
        m_InputActions.Player.Move.canceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        m_InputVector = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        m_InputVector = Vector2.zero;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 targetDir = new Vector3(-m_InputVector.x, 0f, -m_InputVector.y);
        if (targetDir.sqrMagnitude > 1f)
            targetDir.Normalize();

        Vector3 targetVelocity = targetDir * MoveSpeed;
        m_CurrentVelocity = Vector3.SmoothDamp(
            m_CurrentVelocity,
            targetVelocity,
            ref m_SmoothVelocity,
            SmoothTime
        );

        if (m_InputVector == Vector2.zero && m_CurrentVelocity.sqrMagnitude > 0.001f)
        {
            m_CurrentVelocity = Vector3.MoveTowards(m_CurrentVelocity, Vector3.zero, SlideFriction * Time.deltaTime);
        }

        Vector3 move = m_CurrentVelocity;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * m_Agent.angularSpeed
            );
        }

        m_Agent.Move(move * Time.deltaTime);
    }
}
