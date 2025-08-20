using UnityEngine;
using System.Collections;

public class WaterBalloon : Balloon
{
    [Header("Water Balloon")]
    public float Damage = 25f;
    public int WetPower = 20;
    public AnimationCurve PositionCurve;
    public float TotalFlightTime = 1.5f;

    public override void Throw(Vector3 initialVelocity, Vector3? targetPos = null, AnimationCurve curve = null, float totalFlightTime = 1f)
    {
        if (curve != null)
            PositionCurve = curve;
        else if (PositionCurve == null)
            PositionCurve = AnimationCurve.Linear(0, 1, 1, 1);

        if (targetPos.HasValue)
            TotalFlightTime = totalFlightTime;

        base.Throw(initialVelocity, targetPos, curve, totalFlightTime);
    }

    protected override void Update()
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

    protected override void OnCollisionEnter(Collision collision)
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

    public override void Explode()
    {
        if (m_HasExploded) return;
        m_HasExploded = true;

        HideExplosionRadius();

        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                bool isAllyFire = true;
                damageable.TakeDamage(Damage, isAllyFire);
            }

            var wettable = hit.GetComponent<IWettable>();
            if (wettable != null)
            {
                wettable.AddWetness(WetPower);
            }

            var cleanable = hit.GetComponent<ICleanable>();
            if (cleanable != null && !cleanable.IsClean)
            {
                cleanable.Clean();
            }
        }
        Destroy(gameObject);
    }
}