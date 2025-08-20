using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// DashSkill requiere un hitbox (Collider tipo trigger) asignado en el campo DashHitbox para que el dash tenga efectos sobre otros objetos.
/// Si no se asigna un hitbox, el dash solo moverá al jugador, pero no infligirá daño ni interactuará con enemigos u objetos.
/// El hitbox debe ser un GameObject hijo del jugador, estar desactivado por defecto y tener el script DashCollisionHandler.
/// </summary>
public class DashSkill : Skill
{
    [Header("Dash Settings")]
    public float DashDistance = 5f;
    public float DashCooldown = 1f;
    public float DashSpeed = 50f;
    public float DashDuration = 0.2f;

    [Header("Dash Animation Curve")]
    public bool UseDashCurve = false;

    [SerializeField, Tooltip("Curva de velocidad para el dash (0-1 en X, velocidad relativa en Y)")]
    public AnimationCurve DashCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [Header("Dash Hitbox")]
    [Tooltip("Referencia al collider trigger que usará el dash para hacer daño. Debe estar desactivado por defecto.")]
    public Collider DashHitbox;

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
        var prop = so.FindProperty("DashCurve");
        prop.isExpanded = UseDashCurve;
        so.ApplyModifiedProperties();
    }
#endif

    private bool m_IsDashing;
    public bool IsDashing => m_IsDashing;

    private NavMeshAgent m_Agent;

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
        m_Agent = m_PlayerMotor.GetAgent();
        InitCooldown(DashCooldown);
        if (DashHitbox != null)
            DashHitbox.enabled = false;
    }

    public void Activate()
    {
        SetCooldown(DashCooldown);
        if (IsCooldownReady && !m_IsDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        m_IsDashing = true;
        StartCooldown();

        // Lanzar evento de inicio de dash
        EventManager.TriggerEvent(new DashStartEvent {});

        // Activar el hitbox de daño
        if (DashHitbox != null)
            DashHitbox.enabled = true;

        Vector3 startPosition = transform.position;
        Vector3 dashDir = transform.forward;
        float safeOffset = 1.1f;

        Vector3 targetPosition = startPosition + dashDir * DashDistance;

        // Raycast para detectar colisiones, pero ignora objetos en la capa "Interactables" (por ejemplo, Sponges)
        if (Physics.Raycast(startPosition, dashDir, out RaycastHit hit, DashDistance))
        {
            int interactablesLayer = LayerMask.NameToLayer("IgnoreDashImpact");
            if (hit.collider.gameObject.layer != interactablesLayer)
            {
                targetPosition = hit.point - dashDir * safeOffset;
            }
        }

        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            m_IsDashing = false;
            if (DashHitbox != null)
                DashHitbox.enabled = false;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < DashDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / DashDuration);
            float curveT = UseDashCurve && DashCurve != null ? DashCurve.Evaluate(t) : t;
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, curveT);
            Vector3 delta = nextPos - transform.position;
            m_Agent.Move(delta);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_Agent.Move(targetPosition - transform.position);

        yield return new WaitForSeconds(DashDuration);
        m_IsDashing = false;

        // Desactivar el hitbox de daño
        if (DashHitbox != null)
            DashHitbox.enabled = false;

        // Lanzar evento de fin de dash
        EventManager.TriggerEvent(new DashEndEvent {});
    }

}

public struct DashStartEvent{}
public struct DashEndEvent{}