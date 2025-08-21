using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    public Collider DashHitbox;

    private bool m_IsDashing;
    private NavMeshAgent m_Agent;
    private Coroutine m_DashCoroutine;

    public bool IsDashing => m_IsDashing;

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
        m_Agent = m_PlayerMotor.GetAgent();
        if (DashHitbox != null)
            DashHitbox.enabled = false;
    }

    private void Awake()
    {
        InitCooldown(DashCooldown);
    }

    public void Activate()
    {
        SetCooldown(DashCooldown);
        if (IsCooldownReady && !m_IsDashing)
        {
            RaiseSkillStarted(); // Notifica inicio para cancelar otras skills
            m_DashCoroutine = StartCoroutine(DashCoroutine());
        }
    }

    public override void Cancel()
    {
        if (m_IsDashing)
        {
            if (m_DashCoroutine != null)
                StopCoroutine(m_DashCoroutine);
            TerminateDash();
        }
    }

    private IEnumerator DashCoroutine()
    {
        m_IsDashing = true;
        StartCooldown();
        EventManager.TriggerEvent(new DashStartEvent { });

        if (DashHitbox != null)
            DashHitbox.enabled = true;

        Vector3 startPosition = transform.position;
        Vector3 dashDir = transform.forward;
        float safeOffset = 1.1f;
        Vector3 targetPosition = startPosition + dashDir * DashDistance;

        if (Physics.Raycast(startPosition, dashDir, out RaycastHit hit, DashDistance))
        {
            int ignoreLayer = LayerMask.NameToLayer("IgnoreDashImpact");
            if (hit.collider.gameObject.layer != ignoreLayer)
                targetPosition = hit.point - dashDir * safeOffset;
        }

        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            TerminateDash();
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < DashDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / DashDuration);
            float curveT = UseDashCurve && DashCurve != null ? DashCurve.Evaluate(t) : t;
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, curveT);
            m_Agent.Move(nextPos - transform.position);
            elapsedTime += Time.deltaTime;
            yield return null;
            if (!m_IsDashing) yield break; // Cancel guard
        }

        m_Agent.Move(targetPosition - transform.position);
        yield return new WaitForSeconds(DashDuration);
        TerminateDash();
    }

    private void TerminateDash()
    {
        m_IsDashing = false;
        if (DashHitbox != null)
            DashHitbox.enabled = false;
        EventManager.TriggerEvent(new DashEndEvent { });
    }
}

public struct DashStartEvent { }
public struct DashEndEvent { }