using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterBalloonSkill : Skill
{
    [Header("Water Balloon Settings")]
    public GameObject WaterBalloonPrefab;
    public float Damage = 25f;
    public float ExplosionDelay = 1.5f;
    public float Cooldown = 2f;
    public float ScatterForce = 10f;
    public float ExplosionRadius = 2.5f;
    public LayerMask TargetLayers;

    [Header("Aiming Settings")]
    public float MinRange = 2f;
    public float MaxRange = 10f;
    public float MaxHeight = 2f;

    [Header("Flight Settings")]
    [SerializeField]
    private float m_FlightTime = 2f;

    [SerializeField]
    private Transform m_SpawnPoint;
    private CircleDrawer m_CircleDrawer;

    // Guarda el último input válido
    private Vector2 m_LastAimInput = Vector2.zero;

    [Header("Curve Settings")]
    public bool UseAnimationCurve = false;

    [SerializeField, Tooltip("Curva de posición-tiempo para el vuelo del globo")]
    public AnimationCurve VelocityCurve = AnimationCurve.Linear(0, 1, 1, 1);

#if UNITY_EDITOR
    // Oculta el campo en el inspector si UseAnimationCurve es false
    private void OnValidate()
    {
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
        var prop = so.FindProperty("VelocityCurve");
        prop.isExpanded = UseAnimationCurve;
        so.ApplyModifiedProperties();
    }
#endif

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
        InitCooldown(Cooldown);
        m_CircleDrawer = m_PlayerMotor.GetComponentInChildren<CircleDrawer>();
        if (m_CircleDrawer == null)
            Debug.LogWarning("CircleDrawer no encontrado en el jugador.");
    }

    public void OnAimingPerformed(Vector2 input)
    {
        Vector2 invertedInput = new Vector2(-input.x, -input.y);
        if (invertedInput.sqrMagnitude > 0.01f)
            m_LastAimInput = invertedInput;

        UpdateAimingVisual(invertedInput);
    }

    public void OnAimingCanceled(Vector2 input)
    {
        if (m_LastAimInput.sqrMagnitude > 0.01f)
        {
            ThrowWaterBalloon(m_LastAimInput);
            m_LastAimInput = Vector2.zero;
        }
        m_CircleDrawer.Hide();
    }

    public void UpdateAimingVisual(Vector2 input)
    {
        if (m_CircleDrawer == null)
            return;

        if (input.sqrMagnitude < 0.01f)
        {
            m_CircleDrawer.Hide();
            return;
        }

        Vector3 direction = new Vector3(input.x, 0, input.y).normalized;
        float range = Mathf.Lerp(MinRange, MaxRange, input.magnitude);
        Vector3 target = GetGroundedTarget(m_SpawnPoint.position + direction * range);

        m_CircleDrawer.DrawCircle(target, ExplosionRadius);

        if (UseAnimationCurve && VelocityCurve != null)
        {
            // Simula la parábola usando la curva
            m_CircleDrawer.DrawParabolaWithCurve(m_SpawnPoint.position, target, VelocityCurve, m_FlightTime, m_CircleDrawer.ParabolicSegments);
        }
        else
        {
            // Simulación física estándar
            float gravity = Mathf.Abs(Physics.gravity.y);
            float maxHeight = m_SpawnPoint.position.y + MaxHeight;
            Vector3 launchVelocity = ParabolicCalculator.CalculateLaunchVelocity(
                m_SpawnPoint.position, target, maxHeight, gravity
            );
            m_CircleDrawer.DrawParabola(
                m_SpawnPoint.position,
                launchVelocity,
                gravity,
                3f
            );
        }
    }

    public void ThrowWaterBalloon(Vector2 aimInput)
    {
        SetCooldown(Cooldown);
        if (!IsCooldownReady)
            return;

        Vector3 direction = new Vector3(aimInput.x, 0, aimInput.y).normalized;
        float range = Mathf.Lerp(MinRange, MaxRange, aimInput.magnitude);
        Vector3 target = GetGroundedTarget(m_SpawnPoint.position + direction * range);

        GameObject balloonObj = GameObject.Instantiate(WaterBalloonPrefab, m_SpawnPoint.position, Quaternion.identity);
        var balloon = balloonObj.GetComponent<WaterBalloon>();
        if (balloon != null)
        {
            balloon.Damage = Damage;
            balloon.ExplosionDelay = ExplosionDelay;
            balloon.ExplosionRadius = ExplosionRadius;
            balloon.TargetLayers = TargetLayers;

            if (UseAnimationCurve && VelocityCurve != null)
            {
                balloon.Throw(Vector3.zero, target, VelocityCurve, m_FlightTime);
            }
            else
            {
                // Usa física normal
                float gravity = Mathf.Abs(Physics.gravity.y);
                float maxHeight = m_SpawnPoint.position.y + MaxHeight;
                Vector3 launchVelocity = ParabolicCalculator.CalculateLaunchVelocity(
                    m_SpawnPoint.position, target, maxHeight, gravity
                );
                balloon.Throw(launchVelocity);
            }
        }
        StartCooldown();
    }

    // Reemplaza GetGroundedTarget por llamada a GroundDetector
    private Vector3 GetGroundedTarget(Vector3 target)
    {
        // Si tienes una máscara de suelo, pásala aquí
        return GroundDetector.GetGroundedPosition(target);
    }
}