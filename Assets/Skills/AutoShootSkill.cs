using UnityEngine;
using System.Collections;
using KBCore.Refs;

[RequireComponent(typeof(WaterBalloonSkill))]
public class AutoShootSkill : MonoBehaviour
{
    [Header("Auto Shoot Settings")]
    [SerializeField, Self]
    private WaterBalloonSkill SkillToUse;
    public float MinInterval = 2f;
    public float MaxInterval = 4f;
    public float AimTime = 0.5f; // Tiempo que mantiene el "apuntado" antes de disparar
    public LayerMask ValidGroundLayers = ~0; // Capas válidas para disparar (ajusta en el inspector)

    private bool isActive = false;

    private Vector2 lastAimInput;
    private Vector3 lastTargetWorld;

    private void OnEnable()
    {
        isActive = true;

        StartCoroutine(AutoShootRoutine());
    }

    private void OnDisable()
    {
        isActive = false;
        StopAllCoroutines();
    }

    private IEnumerator AutoShootRoutine()
    {
        while (isActive)
        {
            float wait = Random.Range(MinInterval, MaxInterval);
            yield return new WaitForSeconds(wait);

            float minRange = SkillToUse.MinRange;
            float maxRange = SkillToUse.MaxRange;

            Vector2 aimInput = Vector2.zero;
            Vector3 targetWorld = Vector3.zero;
            bool foundValid = false;

            // Bucle infinito hasta encontrar un punto válido
            while (!foundValid)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float randomMagnitude = Random.Range(minRange / maxRange, 1f);
                aimInput = randomDir * randomMagnitude;

                Vector3 direction = new Vector3(aimInput.x, 0, aimInput.y).normalized;
                float range = Mathf.Lerp(minRange, maxRange, aimInput.magnitude);
                targetWorld = SkillToUse.transform.position + direction * range + Vector3.up * 5f;

                if (Physics.Raycast(targetWorld, Vector3.down, out RaycastHit hit, 20f, ValidGroundLayers))
                {
                    foundValid = true;
                }
                else
                {
                    yield return null; // Espera un frame antes de volver a intentar
                }
            }

            lastAimInput = aimInput;
            lastTargetWorld = targetWorld;

            // Simula el apuntado
            SkillToUse.OnAimingPerformed(aimInput);

            // Mientras apunta, actualiza la visualización cada frame
            float timer = 0f;
            while (timer < AimTime)
            {
                SkillToUse.UpdateAimingVisual(lastAimInput);
                timer += Time.deltaTime;
                yield return null;
            }

            // Simula el disparo
            SkillToUse.OnAimingCanceled(lastAimInput);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (SkillToUse == null)
            SkillToUse = GetComponent<WaterBalloonSkill>();

        if (SkillToUse != null)
        {
            DrawCircleGizmo(transform.position, SkillToUse.MaxRange, new Color(1f, 0f, 0f, 0.7f));
            DrawCircleGizmo(transform.position, SkillToUse.MinRange, new Color(1f, 0f, 0f, 0.3f));
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