using UnityEngine;
using System.Collections;
using KBCore.Refs;

public class WaterBalloon : MonoBehaviour, IExplodable
{
    [Header("Explosion Settings")]
    public float Damage = 25f;
    public float ExplosionRadius = 2.5f;
    public float ExplosionDelay = 1.5f;
    public LayerMask TargetLayers;
    public LayerMask GroundLayers;

    [Header("Flight Curve (optional)")]
    public AnimationCurve PositionCurve; // Si está vacía, no se usa
    public float TotalFlightTime = 1.5f; // Tiempo total de vuelo (solo si se usa curva)

    protected bool m_HasExploded = false;
    protected bool m_HasTouchedGround = false;
    protected Coroutine m_ExplosionCoroutine;

    public bool HasExploded => m_HasExploded;
    public bool HasTouchedGround => m_HasTouchedGround;

    [SerializeField, Self]
    protected Rigidbody rb;

    // Variables para la curva
    private Vector3 m_StartPos;
    private Vector3 m_TargetPos;
    private float m_Lifetime = 0f;
    private Vector3 m_InitialVelocity;
    private bool m_UseCurve = false;

    /// <summary>
    /// Lanza el globo con una velocidad inicial (física) o con una curva de posición-tiempo.
    /// </summary>
    public void Throw(Vector3 initialVelocity, Vector3? targetPos = null, AnimationCurve curve = null, float totalFlightTime = 1f)
    {
        m_UseCurve = (curve != null && targetPos.HasValue);
        m_Lifetime = 0f;

        if (m_UseCurve)
        {
            m_StartPos = transform.position;
            m_TargetPos = targetPos.Value;
            PositionCurve = curve;
            TotalFlightTime = totalFlightTime;
            if (rb != null)
                rb.isKinematic = true; // Movimiento manual
        }
        else
        {
            m_InitialVelocity = initialVelocity;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = m_InitialVelocity;
            }
        }
    }

    protected virtual void Update()
    {
        if (!m_HasTouchedGround && m_UseCurve)
        {
            m_Lifetime += Time.deltaTime;
            float tNorm = Mathf.Clamp01(m_Lifetime / TotalFlightTime);
            float curveT = PositionCurve != null ? PositionCurve.Evaluate(tNorm) : tNorm;

            // Interpolación parabólica entre start y target usando la curva
            Vector3 pos = ParabolicLerp(m_StartPos, m_TargetPos, curveT);

            // Comprobar si hemos llegado al suelo
            if (tNorm >= 1f)
            {
                // Raycast hacia abajo para encontrar el suelo
                if (Physics.Raycast(pos + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 1f, GroundLayers))
                {
                    transform.position = hit.point;
                }
                else
                {
                    transform.position = pos;
                }

                m_HasTouchedGround = true;
                m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
            }
            else
            {
                transform.position = pos;
            }
        }
    }

    // Interpolación parabólica entre dos puntos (puedes ajustar la altura máxima si quieres)
    private Vector3 ParabolicLerp(Vector3 start, Vector3 end, float t)
    {
        float height = Mathf.Max(start.y, end.y) + 3f; // Altura máxima, ajustable
        // Trayectoria parabólica simple
        Vector3 mid = Vector3.Lerp(start, end, t);
        float parabola = 4 * height * t * (1 - t);
        mid.y += parabola;
        return mid;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded)
            return;

        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            m_HasTouchedGround = true;
            m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }

    protected IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        Explode();
    }

    public void Explode()
    {
        if (m_HasExploded) return;
        m_HasExploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(Damage);
            }
        }
        Destroy(gameObject);
    }
}