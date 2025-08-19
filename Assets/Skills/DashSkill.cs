using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
    // Expande o colapsa la curva según el booleano, igual que WaterBalloonSkill
    private void OnValidate()
    {
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
        var prop = so.FindProperty("DashCurve");
        prop.isExpanded = UseDashCurve;
        so.ApplyModifiedProperties();
    }
#endif

    private bool m_IsDashing;
    private NavMeshAgent m_Agent;

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
        m_Agent = m_PlayerMotor.GetAgent();
        InitCooldown(DashCooldown);
    }

    public void Activate()
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

        // Raycast para detectar colisiones, pero ignora objetos en la capa "Interactables" (por ejemplo, Sponges)
        if (Physics.Raycast(startPosition, dashDir, out RaycastHit hit, DashDistance))
        {
            // Comprobamos si el objeto impactado está en la capa "Interactables"
            int interactablesLayer = LayerMask.NameToLayer("IgnoreDashImpact");
            if (hit.collider.gameObject.layer != interactablesLayer)
            {
                targetPosition = hit.point - dashDir * safeOffset;
            }
            // Si es Interactables (por ejemplo, Sponge), el dash atraviesa y no se acorta
        }

        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            m_IsDashing = false;
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
    }

    public bool IsDashing => m_IsDashing;
}