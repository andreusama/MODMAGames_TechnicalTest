using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DashSkill : MonoBehaviour
{
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    private NavMeshAgent agent;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashCooldown = 1f;
    public float dashSpeed = 50f;

    private bool isDashing;
    private float cooldownTime;

    public InputMap inputActions;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (Time.time >= cooldownTime && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        cooldownTime = Time.time + dashCooldown;

        Vector3 startPosition = transform.position;
        Vector3 dashDir = transform.forward;
        float safeOffset = 1.1f; // Ajusta según el tamaño de tu jugador

        // Por defecto, el destino es la distancia máxima de dash
        Vector3 targetPosition = startPosition + dashDir * dashDistance;

        // Raycast para detectar el primer obstáculo físico en línea recta
        if (Physics.Raycast(startPosition, dashDir, out RaycastHit hit, dashDistance))
        {
            // Ajusta el destino al punto de colisión menos el offset de seguridad
            targetPosition = hit.point - dashDir * safeOffset;
        }

        // Si el destino es igual al inicio, no hay dash posible
        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            isDashing = false;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            Vector3 delta = nextPos - transform.position;
            agent.Move(delta);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        agent.Move(targetPosition - transform.position);

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
}
