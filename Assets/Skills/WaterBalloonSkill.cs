using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System; // añadido para eventos

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
    public CircleDrawer m_CircleDrawer;

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

    [Header("Aiming Slowdown")]
    [Range(0.1f, 1f)]
    public float AimingSpeedPercent = 0.4f; // 40% de la velocidad original

    private float? originalMoveSpeed = null;
    private bool m_IsAiming = false;
    public bool IsAiming => m_IsAiming; // NUEVO: expone si está apuntando
    public event Action OnShot;          // NUEVO: evento cuando se lanza el globo

    public override void Initialize(PlayerMotor motor)
    {
        base.Initialize(motor);
    }

    private void Awake()
    {
        InitCooldown(Cooldown);
    }

    public void OnAimingPerformed(Vector2 input)
    {
        Vector2 invertedInput = new Vector2(-input.x, -input.y);

        if (invertedInput.sqrMagnitude > 0.01f)
        {
            m_LastAimInput = invertedInput;
            if (!m_IsAiming)
            {
                m_IsAiming = true;
                RaiseSkillStarted(); // Notifica inicio (cancelará otras skills)
            }
        }

        if (m_IsAiming && originalMoveSpeed == null && m_PlayerMotor != null)
        {
            originalMoveSpeed = m_PlayerMotor.MoveSpeed;
            m_PlayerMotor.MoveSpeed = originalMoveSpeed.Value * AimingSpeedPercent;
        }

        UpdateAimingVisual(invertedInput);
    }

    public void OnAimingCanceled(Vector2 input, Vector3? fixedTarget = null)
    {
        m_CircleDrawer?.Hide();

        if (m_LastAimInput.sqrMagnitude > 0.01f)
        {
            SetCooldown(Cooldown);
            if (IsCooldownReady)
            {
                ThrowWaterBalloon(m_LastAimInput, fixedTarget);
                OnShot?.Invoke(); // NUEVO: notifica disparo
                StartCooldown();
            }
        }

        ResetAimingState();
    }

    public override void Cancel()
    {
        // Cancelación sin disparo
        m_CircleDrawer?.Hide();
        ResetAimingState();
    }

    private void ResetAimingState()
    {
        if (!m_IsAiming && originalMoveSpeed == null) return;

        m_IsAiming = false;
        m_LastAimInput = Vector2.zero;

        if (originalMoveSpeed.HasValue && m_PlayerMotor != null)
        {
            m_PlayerMotor.MoveSpeed = originalMoveSpeed.Value;
            originalMoveSpeed = null;
        }
    }

    public void UpdateAimingVisual(Vector2 input, Vector3? fixedTarget = null)
    {
        if (m_CircleDrawer == null)
            return;

        if (input.sqrMagnitude < 0.01f)
        {
            m_CircleDrawer.Hide();
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

        m_CircleDrawer.DrawCircle(target, ExplosionRadius);

        if (UseAnimationCurve && VelocityCurve != null)
        {
            m_CircleDrawer.DrawParabolaWithCurve(origin, target, VelocityCurve, m_FlightTime, m_CircleDrawer.ParabolicSegments);
        }
        else
        {
            float gravity = Mathf.Abs(Physics.gravity.y);
            float maxHeight = origin.y + MaxHeight;
            Vector3 launchVelocity = ParabolicCalculator.CalculateLaunchVelocity(origin, target, maxHeight, gravity);
            m_CircleDrawer.DrawParabola(origin, launchVelocity, gravity, 3f);
        }
    }

    public void ThrowWaterBalloon(Vector2 aimInput, Vector3? fixedTarget = null)
    {
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