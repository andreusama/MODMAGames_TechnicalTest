using UnityEngine;
using KBCore.Refs;
using System.Collections;
using MoreMountains.Feedbacks;
using System;

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

    [Header("Grounding")]
    [Tooltip("Extra elevation above ground to avoid z-fighting and particles under the floor")]
    [SerializeField] private float m_GroundClearance = 0.02f;

    [Header("Feedbacks (MMFeedbacks)")]
    [Tooltip("Feedbacks played on ground touch")]
    [SerializeField] private MMF_Player m_GroundTouchFeedback;
    [Tooltip("Feedbacks played on explosion")]
    [SerializeField] private MMF_Player m_ExplosionFeedback;

    protected bool m_HasExploded = false;
    protected bool m_HasTouchedGround = false;

    public bool HasExploded => m_HasExploded;
    public bool HasTouchedGround => m_HasTouchedGround;

    [SerializeField, Self] protected Rigidbody m_Rigidbody;
    protected CircleDrawer m_CircleDrawer;
    protected Collider m_Collider;

    // Curve variables
    protected Vector3 m_StartPos;
    protected Vector3 m_TargetPos;
    protected float m_Lifetime = 0f;
    protected bool m_UseCurve = false;

    public event Action<Balloon> TouchedGround;
    public event Action<Balloon> Exploded;

    protected virtual void Awake()
    {
        m_CircleDrawer = GetComponentInChildren<CircleDrawer>();
        m_Collider = GetComponent<Collider>();

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
            if (m_Rigidbody != null) m_Rigidbody.isKinematic = true; // manual movement
        }
        else
        {
            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.linearVelocity = initialVelocity;
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
                Vector3 ground = GroundDetector.GetGroundedPosition(pos, 0.2f, 1f, GroundLayers);
                float lift = GetHalfHeightWorld() + m_GroundClearance;
                transform.position = ground + Vector3.up * lift;
                HandleTouchedGround();
            }
            else
            {
                transform.position = pos;
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded) return;

        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            HandleTouchedGround(collision);
        }
    }

    // Hook for derived classes. collision can be null (curve/Update case).
    // Base implementation: reposition above ground if coming from collision.
    protected virtual void OnTouchedGroundHook(Collision collision)
    {
        if (collision != null && collision.contacts.Length > 0)
        {
            var cp = collision.contacts[0];
            float lift = GetHalfHeightWorld() + m_GroundClearance;
            transform.position = cp.point + cp.normal * lift;
        }
    }

    protected void HandleTouchedGround(Collision collision = null)
    {
        if (m_HasTouchedGround) return;
        m_HasTouchedGround = true;

        OnTouchedGroundHook(collision);

        ShowExplosionRadius();
        TriggerGroundTouchFeedbacks();
        StartCoroutine(ExplodeAfterDelay());
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
        if (m_HasExploded) return;
        m_HasExploded = true;

        HideExplosionRadius();
        TriggerExplosionFeedbacks();
        StartCoroutine(DestroyCoroutine());
    }

    public IEnumerator DestroyCoroutine()
    {
        if (m_ExplosionFeedback != null)
            yield return m_ExplosionFeedback.IsPlaying ? new WaitUntil(() => !m_ExplosionFeedback.IsPlaying) : null;

        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        HideExplosionRadius();
    }

    protected void TriggerGroundTouchFeedbacks()
    {
        if (m_GroundTouchFeedback != null)
            m_GroundTouchFeedback.PlayFeedbacks(transform.position);

        TouchedGround?.Invoke(this);
    }

    protected void TriggerExplosionFeedbacks()
    {
        if (m_ExplosionFeedback != null)
            m_ExplosionFeedback.PlayFeedbacks(transform.position);

        Exploded?.Invoke(this);
    }

    // World half height from collider (robust for sphere, capsule, box, etc.)
    protected float GetHalfHeightWorld()
    {
        return (m_Collider != null) ? m_Collider.bounds.extents.y : 0f;
    }
}