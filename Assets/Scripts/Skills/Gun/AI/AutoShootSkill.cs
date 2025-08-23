using UnityEngine;
using System.Collections;
using KBCore.Refs;

[RequireComponent(typeof(BalloonGunSkill))]
public class AutoShootSkill : MonoBehaviour
{
    [Header("Auto Shoot Settings")]
    [SerializeField, Self]
    private BalloonGunSkill m_SkillToUse;
    public float MinInterval = 2f;
    public float MaxInterval = 4f;
    public float AimTime = 0.5f; // Time to keep "aiming" before shooting
    public LayerMask ValidGroundLayers = ~0; // Valid layers to shoot at (tune in inspector)

    private bool m_IsActive = false;

    private Vector2 m_LastAimInput;
    private Vector3 m_LastTargetWorld;

    private void OnEnable()
    {
        m_IsActive = true;
        StartCoroutine(AutoShootRoutine());
    }

    private void OnDisable()
    {
        m_IsActive = false;
        StopAllCoroutines();
    }

    private IEnumerator AutoShootRoutine()
    {
        while (m_IsActive)
        {
            float wait = Random.Range(MinInterval, MaxInterval);
            yield return new WaitForSeconds(wait);

            float minRange = m_SkillToUse.MinRange;
            float maxRange = m_SkillToUse.MaxRange;

            Vector2 aimInput = Vector2.zero;
            Vector3 targetWorld = Vector3.zero;
            bool foundValid = false;

            // Loop until finding a valid point
            while (!foundValid)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float randomMagnitude = Random.Range(minRange / maxRange, 1f);
                aimInput = randomDir * randomMagnitude;

                Vector3 direction = new Vector3(aimInput.x, 0, aimInput.y).normalized;
                float range = Mathf.Lerp(minRange, maxRange, aimInput.magnitude);
                Vector3 rayOrigin = m_SkillToUse.transform.position + direction * range + Vector3.up * 5f;

                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 20f, ValidGroundLayers))
                {
                    foundValid = true;
                    targetWorld = hit.point; // Store real impact point as absolute target
                }
                else
                {
                    yield return null; // Wait one frame before trying again
                }
            }

            m_LastAimInput = aimInput;
            m_LastTargetWorld = targetWorld;

            // Simulate aiming
            m_SkillToUse.OnAimingPerformed(aimInput);

            // While aiming, update visuals every frame with a fixed target
            float timer = 0f;
            while (timer < AimTime)
            {
                m_SkillToUse.UpdateAimingVisual(m_LastAimInput, m_LastTargetWorld);
                timer += Time.deltaTime;
                yield return null;
            }

            // Simulate shooting using the fixed target
            m_SkillToUse.OnAimingCanceled(m_LastAimInput, m_LastTargetWorld);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_SkillToUse == null)
            m_SkillToUse = GetComponent<BalloonGunSkill>();

        if (m_SkillToUse != null)
        {
            DrawCircleGizmo(transform.position, m_SkillToUse.MaxRange, new Color(1f, 0f, 0f, 0.7f));
            DrawCircleGizmo(transform.position, m_SkillToUse.MinRange, new Color(1f, 0f, 0f, 0.3f));
        }
    }

    private void DrawCircleGizmo(Vector3 center, float radius, Color color, int segments = 64)
    {
        Gizmos.color = color;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
#endif
}