using System.Collections;
using UnityEngine;
using System; // added for events

public class BalloonGunSkill : Skill
{
    [Header("Config")]
    [SerializeField] private BalloonGunConfiguration m_Config;

    [Header("Water Balloon Settings")]
    public GameObject WaterBalloonPrefab;
    public float ExplosionDelay;
    public float Cooldown;
    public float ExplosionRadius;
    public LayerMask TargetLayers;

    [Header("Aiming Settings")]
    public float MinRange;
    public float MaxRange;
    public float MaxHeight;

    [Header("Flight Settings")]
    [SerializeField]
    private float m_FlightTime;

    [SerializeField]
    private Transform m_SpawnPoint;
    public CircleDrawer CircleDrawer;

    // Stores last valid input
    private Vector2 m_LastAimInput = Vector2.zero;

    [Header("Curve Settings")]
    public bool UseAnimationCurve = false;

    [SerializeField, Tooltip("Position-time curve for balloon flight")]
    public AnimationCurve VelocityCurve = AnimationCurve.Linear(0, 1, 1, 1);

#if UNITY_EDITOR
    // Hide field in inspector if UseAnimationCurve is false
    private void OnValidate()
    {
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
        var prop = so.FindProperty("VelocityCurve");
        prop.isExpanded = UseAnimationCurve;
        so.ApplyModifiedProperties();
    }
#endif

    [Header("Aiming Slowdown")]
    [Range(0.1f, 1f)]
    public float AimingSpeedPercent; // from config

    private float? m_OriginalMoveSpeed = null;
    private bool m_IsAiming = false;
    public bool IsAiming => m_IsAiming; // exposes if aiming
    public event Action OnShot;          // event when the balloon is thrown

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
    }

    private void Awake()
    {
        if (m_Config == null)
        {
            Debug.LogWarning($"{name}: BalloonGunConfiguration no asignado. Deshabilitando habilidad para evitar interacción del jugador.");
            enabled = false;
            return;
        }

        // Cargar configuración
        WaterBalloonPrefab = m_Config.WaterBalloonPrefab;
        ExplosionDelay = m_Config.ExplosionDelay;
        Cooldown = m_Config.Cooldown;
        ExplosionRadius = m_Config.ExplosionRadius;
        TargetLayers = m_Config.TargetLayers;

        MinRange = m_Config.MinRange;
        MaxRange = m_Config.MaxRange;
        MaxHeight = m_Config.MaxHeight;
        AimingSpeedPercent = m_Config.AimingSpeedPercent;

        m_FlightTime = m_Config.FlightTime;
        UseAnimationCurve = m_Config.UseAnimationCurve;
        VelocityCurve = m_Config.VelocityCurve;

        InitCooldown(Cooldown);
    }

    public void OnAimingPerformed(Vector2 input)
    {
        if (!enabled) return;

        Vector2 invertedInput = new Vector2(-input.x, -input.y);

        if (invertedInput.sqrMagnitude > 0.01f)
        {
            m_LastAimInput = invertedInput;
            if (!m_IsAiming)
            {
                m_IsAiming = true;
                RaiseSkillStarted(); // Notify start (will cancel other skills)
            }
        }

        if (m_IsAiming && m_OriginalMoveSpeed == null && m_PlayerMotor != null)
        {
            m_OriginalMoveSpeed = m_PlayerMotor.MoveSpeed;
            m_PlayerMotor.MoveSpeed = m_OriginalMoveSpeed.Value * AimingSpeedPercent;
        }

        UpdateAimingVisual(invertedInput);
    }

    public void OnAimingCanceled(Vector2 input, Vector3? fixedTarget = null)
    {
        if (!enabled) return;

        CircleDrawer?.Hide();

        if (m_LastAimInput.sqrMagnitude > 0.01f)
        {
            SetCooldown(Cooldown);
            if (IsCooldownReady)
            {
                ThrowWaterBalloon(m_LastAimInput, fixedTarget);
                OnShot?.Invoke(); // notify shot
                StartCooldown();
            }
        }

        ResetAimingState();
    }

    public override void Cancel()
    {
        // Cancel without shooting
        CircleDrawer?.Hide();
        ResetAimingState();
    }

    private void ResetAimingState()
    {
        if (!m_IsAiming && m_OriginalMoveSpeed == null) return;

        m_IsAiming = false;
        m_LastAimInput = Vector2.zero;

        if (m_OriginalMoveSpeed.HasValue && m_PlayerMotor != null)
        {
            m_PlayerMotor.MoveSpeed = m_OriginalMoveSpeed.Value;
            m_OriginalMoveSpeed = null;
        }
    }

    public void UpdateAimingVisual(Vector2 input, Vector3? fixedTarget = null)
    {
        if (!enabled || CircleDrawer == null)
            return;

        if (input.sqrMagnitude < 0.01f)
        {
            CircleDrawer.Hide();
            return;
        }

        Vector3 origin = m_SpawnPoint.position;
        Vector3 target;

        if (fixedTarget.HasValue)
        {
            target = fixedTarget.Value;
        }
        else
        {
            Vector3 direction = new Vector3(input.x, 0, input.y).normalized;
            float range = Mathf.Lerp(MinRange, MaxRange, input.magnitude);
            target = GetGroundedTarget(origin + direction * range);
        }

        CircleDrawer.DrawCircle(target, ExplosionRadius);

        if (UseAnimationCurve && VelocityCurve != null)
        {
            CircleDrawer.DrawParabolaWithCurve(origin, target, VelocityCurve, m_FlightTime, CircleDrawer.ParabolicSegments);
        }
        else
        {
            float gravity = Mathf.Abs(Physics.gravity.y);
            float maxHeight = origin.y + MaxHeight;
            Vector3 launchVelocity = ParabolicCalculator.CalculateLaunchVelocity(origin, target, maxHeight, gravity);
            CircleDrawer.DrawParabola(origin, launchVelocity, gravity, 3f);
        }
    }

    public void ThrowWaterBalloon(Vector2 aimInput, Vector3? fixedTarget = null)
    {
        if (!enabled) return;

        Vector3 origin = m_SpawnPoint.position;
        Vector3 target;

        if (fixedTarget.HasValue)
        {
            target = fixedTarget.Value;
        }
        else
        {
            Vector3 direction = new Vector3(aimInput.x, 0, aimInput.y).normalized;
            float range = Mathf.Lerp(MinRange, MaxRange, aimInput.magnitude);
            target = GetGroundedTarget(origin + direction * range);
        }

        GameObject balloonObj = Instantiate(WaterBalloonPrefab, origin, Quaternion.identity);
        var balloon = balloonObj.GetComponent<Balloon>();
        if (balloon != null)
        {
            balloon.ExplosionDelay = ExplosionDelay;
            balloon.ExplosionRadius = ExplosionRadius;
            balloon.TargetLayers = TargetLayers;

            if (UseAnimationCurve && VelocityCurve != null)
            {
                balloon.Throw(Vector3.zero, target, VelocityCurve, m_FlightTime);
            }
            else
            {
                float gravity = Mathf.Abs(Physics.gravity.y);
                float maxHeight = origin.y + MaxHeight;
                Vector3 launchVelocity = ParabolicCalculator.CalculateLaunchVelocity(origin, target, maxHeight, gravity);
                balloon.Throw(launchVelocity);
            }
        }
    }

    private Vector3 GetGroundedTarget(Vector3 target)
    {
        return GroundDetector.GetGroundedPosition(target);
    }
}