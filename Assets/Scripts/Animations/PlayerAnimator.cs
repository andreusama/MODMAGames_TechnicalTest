using KBCore.Refs;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour, IEventListener<DashStartEvent>, IEventListener<DashEndEvent>
{
    [Header("Refs")]
    [SerializeField, Self] private PlayerMotor m_Motor;
    [SerializeField] private SkillManager m_SkillManager;

    [Header("Animation")]
    [SerializeField, Child] private Animator m_Animator;
    [SerializeField] private string m_AnimParamIsRunning = "IsRunning";
    [SerializeField] private string m_AnimParamIsAiming = "IsAiming";
    [SerializeField] private string m_AnimTriggerShoot = "Shoot";
    [SerializeField] private string m_AnimTriggerDash = "Dash";

    [Header("Tuning")]
    [Tooltip("Minimum speed (m/s) to consider running.")]
    public float RunSpeedThreshold = 0.1f;
    [Tooltip("Time to keep in SHOOT state before re-evaluating (seconds).")]
    public float ShootLockTime = 0.35f;

    private enum PlayerState { Idle, Running, Aiming, Shoot, Dash }
    private PlayerState m_State = PlayerState.Idle;

    private BalloonGunSkill m_WaterBalloon;
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
            m_WaterBalloon = m_SkillManager.GetSkill<BalloonGunSkill>();
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
        // If we are DASHING, let the animation handle it until it finishes
        if (m_State == PlayerState.Dash) return;

        // Short lock for shoot animation
        if (m_State == PlayerState.Shoot && Time.time < m_ShootUnlockTime) return;

        // AIMING state has priority over locomotion
        if (m_WaterBalloon != null && m_WaterBalloon.IsAiming)
        {
            SetState(PlayerState.Aiming);
            return;
        }

        // Determine speed from actual displacement (independent from input)
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
        // When dash finishes, re-evaluate immediately in Update
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
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                    m_Animator.SetBool(m_AnimParamIsAiming, false);
                }
                break;

            case PlayerState.Running:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(m_AnimParamIsRunning, true);
                    m_Animator.SetBool(m_AnimParamIsAiming, false);
                }
                break;

            case PlayerState.Aiming:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                    m_Animator.SetBool(m_AnimParamIsAiming, true);
                }
                break;

            case PlayerState.Shoot:
                if (m_Animator != null)
                {
                    // Usually after SHOOT it will return via Has Exit Time to the previous state
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                    m_Animator.SetBool(m_AnimParamIsAiming, false);
                    if (!string.IsNullOrEmpty(m_AnimTriggerShoot))
                        m_Animator.SetTrigger(m_AnimTriggerShoot);
                }
                break;

            case PlayerState.Dash:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                    m_Animator.SetBool(m_AnimParamIsAiming, false);
                    if (!string.IsNullOrEmpty(m_AnimTriggerDash))
                        m_Animator.SetTrigger(m_AnimTriggerDash);
                }
                break;
        }
    }
}