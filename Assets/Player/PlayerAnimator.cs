using KBCore.Refs;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour, IEventListener<DashStartEvent>, IEventListener<DashEndEvent>
{
    [Header("Refs")]
    [SerializeField, Self] private PlayerMotor m_Motor;
    [SerializeField] private SkillManager m_SkillManager;

    [Header("Animation")]
    [SerializeField, Child] private Animator m_Animator;
    [SerializeField] private string AnimParamIsRunning = "IsRunning";
    [SerializeField] private string AnimParamIsAiming = "IsAiming";
    [SerializeField] private string AnimTriggerShoot = "Shoot";
    [SerializeField] private string AnimTriggerDash = "Dash";

    [Header("Tuning")]
    [Tooltip("Velocidad mínima (m/s) para considerar que está corriendo.")]
    public float RunSpeedThreshold = 0.1f;
    [Tooltip("Tiempo durante el que se mantiene en estado SHOOT antes de reevaluar (segundos).")]
    public float ShootLockTime = 0.35f;

    private enum PlayerState { Idle, Running, Aiming, Shoot, Dash }
    private PlayerState m_State = PlayerState.Idle;

    private WaterBalloonSkill m_WaterBalloon;
    private DashSkill m_Dash;

    private Vector3 m_LastPos;
    private float m_ShootUnlockTime;

    private void Awake()
    {
        if (m_Motor == null) m_Motor = GetComponent<PlayerMotor>();
        if (m_Animator == null) m_Animator = GetComponentInChildren<Animator>();

        if (m_SkillManager == null) m_SkillManager = GetComponentInChildren<SkillManager>();
        if (m_SkillManager != null)
        {
            m_WaterBalloon = m_SkillManager.GetSkill<WaterBalloonSkill>();
            m_Dash = m_SkillManager.GetSkill<DashSkill>();
        }
    }

    private void OnEnable()
    {
        this.EventStartListening<DashStartEvent>();
        this.EventStartListening<DashEndEvent>();

        if (m_WaterBalloon != null)
            m_WaterBalloon.OnShot += HandleShot;
    }

    private void OnDisable()
    {
        this.EventStopListening<DashStartEvent>();
        this.EventStopListening<DashEndEvent>();

        if (m_WaterBalloon != null)
            m_WaterBalloon.OnShot -= HandleShot;
    }

    private void Start()
    {
        m_LastPos = transform.position;
        SetState(PlayerState.Idle);
    }

    private void Update()
    {
        // Si estamos en DASH, dejamos que la animación lo gestione hasta que termine
        if (m_State == PlayerState.Dash) return;

        // Bloque corto para la animación de disparo
        if (m_State == PlayerState.Shoot && Time.time < m_ShootUnlockTime) return;

        // Estado AIMING tiene prioridad sobre locomoción
        if (m_WaterBalloon != null && m_WaterBalloon.IsAiming)
        {
            SetState(PlayerState.Aiming);
            return;
        }

        // Determina velocidad por desplazamiento real (independiente del input)
        Vector3 move = m_Motor.GetCurrentVelocity();
        if (move.sqrMagnitude > 0.001f)
            SetState(PlayerState.Running);
        else
            SetState(PlayerState.Idle);
    }

    private void HandleShot()
    {
        SetState(PlayerState.Shoot);
        m_ShootUnlockTime = Time.time + ShootLockTime;
    }

    public void OnEvent(DashStartEvent evt)
    {
        SetState(PlayerState.Dash);
    }

    public void OnEvent(DashEndEvent evt)
    {
        // Al terminar el dash, reevaluamos inmediatamente en Update
        if (m_State == PlayerState.Dash)
            SetState(PlayerState.Idle);
    }

    private void SetState(PlayerState newState)
    {
        if (m_State == newState) return;
        m_State = newState;

        switch (m_State)
        {
            case PlayerState.Idle:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(AnimParamIsRunning, false);
                    m_Animator.SetBool(AnimParamIsAiming, false);
                }
                break;

            case PlayerState.Running:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(AnimParamIsRunning, true);
                    m_Animator.SetBool(AnimParamIsAiming, false);
                }
                break;

            case PlayerState.Aiming:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(AnimParamIsRunning, false);
                    m_Animator.SetBool(AnimParamIsAiming, true);
                }
                break;

            case PlayerState.Shoot:
                if (m_Animator != null)
                {
                    // Normalmente tras SHOOT volverá por Has Exit Time al estado previo
                    m_Animator.SetBool(AnimParamIsRunning, false);
                    m_Animator.SetBool(AnimParamIsAiming, false);
                    if (!string.IsNullOrEmpty(AnimTriggerShoot))
                        m_Animator.SetTrigger(AnimTriggerShoot);
                }
                break;

            case PlayerState.Dash:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(AnimParamIsRunning, false);
                    m_Animator.SetBool(AnimParamIsAiming, false);
                    if (!string.IsNullOrEmpty(AnimTriggerDash))
                        m_Animator.SetTrigger(AnimTriggerDash);
                }
                break;
        }
    }
}