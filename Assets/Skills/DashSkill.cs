using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

public class DashSkill : Skill
{
    [Header("Dash Settings")]
    public float DashDistance = 5f;
    public float DashCooldown = 1f;
    public float DashSpeed = 50f;
    public float DashDuration = 0.2f;

    private bool m_IsDashing;
    private NavMeshAgent m_Agent;

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
        m_Agent = m_PlayerMotor.GetAgent();
        InitCooldown(DashCooldown);
    }

    public override void BindInput(InputMap actions)
    {
        actions.Player.Dash.performed += OnDashPerformed;
        actions.Player.Dash.Enable();
    }

    public override void UnbindInput(InputMap actions)
    {
        actions.Player.Dash.performed -= OnDashPerformed;
        actions.Player.Dash.Disable();
    }

    public void OnDashPerformed(InputAction.CallbackContext context)
    {
        SetCooldown(DashCooldown); // Por si se cambia en runtime
        if (IsCooldownReady && !m_IsDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        m_IsDashing = true;
        StartCooldown();

        Vector3 startPosition = transform.position;
        Vector3 dashDir = transform.forward;
        float safeOffset = 1.1f;

        Vector3 targetPosition = startPosition + dashDir * DashDistance;

        if (Physics.Raycast(startPosition, dashDir, out RaycastHit hit, DashDistance))
        {
            targetPosition = hit.point - dashDir * safeOffset;
        }

        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            m_IsDashing = false;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < DashDuration)
        {
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, elapsedTime / DashDuration);
            Vector3 delta = nextPos - transform.position;
            m_Agent.Move(delta);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_Agent.Move(targetPosition - transform.position);

        yield return new WaitForSeconds(DashDuration);
        m_IsDashing = false;
    }
}