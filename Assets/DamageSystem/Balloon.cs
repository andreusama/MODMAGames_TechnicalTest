using UnityEngine;
using KBCore.Refs;
using System.Collections;

public class Balloon : MonoBehaviour, IExplodable
{
    [Header("Balloon Settings")]
    public float ExplosionRadius = 2.5f;
    public float ExplosionDelay = 1.5f;
    public LayerMask TargetLayers;
    public LayerMask GroundLayers;

    [Header("Parabolic Flight")]
    public AnimationCurve PositionCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public float TotalFlightTime = 1.5f;

    protected bool m_HasExploded = false;
    protected bool m_HasTouchedGround = false;
    protected Coroutine m_ExplosionCoroutine;

    public bool HasExploded => m_HasExploded;
    public bool HasTouchedGround => m_HasTouchedGround;

    [SerializeField, Self]
    protected Rigidbody rb;

    protected CircleDrawer m_CircleDrawer;

    // Variables para la curva
    protected Vector3 m_StartPos;
    protected Vector3 m_TargetPos;
    protected float m_Lifetime = 0f;
    protected Vector3 m_InitialVelocity;
    protected bool m_UseCurve = false;

    protected virtual void Awake()
    {
        m_CircleDrawer = GetComponentInChildren<CircleDrawer>();
        if (m_CircleDrawer == null)
        {
            var go = new GameObject("CircleDrawer");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            m_CircleDrawer = go.AddComponent<CircleDrawer>();
        }
    }

    public virtual void Throw(Vector3 initialVelocity, Vector3? targetPos = null, AnimationCurve curve = null, float totalFlightTime = 1f)
    {
        m_UseCurve = (curve != null && targetPos.HasValue);
        m_Lifetime = 0f;

        if (m_UseCurve)
        {
            m_StartPos = transform.position;
            m_TargetPos = targetPos.Value;
            PositionCurve = curve ?? PositionCurve;
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

            Vector3 pos = ParabolicCalculator.ParabolicLerp(m_StartPos, m_TargetPos, curveT);

            if (tNorm >= 1f)
            {
                transform.position = GroundDetector.GetGroundedPosition(pos, 0.2f, 1f, GroundLayers);
                m_HasTouchedGround = true;
                ShowExplosionRadius();
                m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
            }
            else
            {
                transform.position = pos;
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded)
            return;

        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            m_HasTouchedGround = true;
            ShowExplosionRadius();
            m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }

    protected IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        Explode();
    }

    protected void ShowExplosionRadius()
    {
        if (m_CircleDrawer != null)
            m_CircleDrawer.DrawCircle(transform.position, ExplosionRadius);
    }

    protected void HideExplosionRadius()
    {
        if (m_CircleDrawer != null)
            m_CircleDrawer.Hide();
    }

    public virtual void Explode()
    {
        m_HasExploded = true;
        HideExplosionRadius();
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        HideExplosionRadius();
    }
}