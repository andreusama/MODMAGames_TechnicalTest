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
    [Tooltip("Time to keep in SHOOT state before re-evaluating (seconds). (Does not block locomotion)")]
    public float ShootLockTime = 0.35f;

    private enum LocomotionState { Idle, Running, Dash }
    private LocomotionState m_Locomotion = LocomotionState.Idle;

    private BalloonGunSkill m_WaterBalloon;

    [SerializeField] private ParticleSystem m_RunParticles;

    private void Awake()
    {
        if (m_Motor == null) m_Motor = GetComponent<PlayerMotor>();
        if (m_Animator == null) m_Animator = GetComponentInChildren<Animator>();

        if (m_SkillManager == null) m_SkillManager = GetComponentInChildren<SkillManager>();
        if (m_SkillManager != null)
        {
            m_WaterBalloon = m_SkillManager.GetSkill<BalloonGunSkill>();
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
        ApplyLocomotion(LocomotionState.Idle);
        ApplyAiming(false);
    }

    private void Update()
    {
        // 1) Locomotion
        if (m_Locomotion != LocomotionState.Dash)
        {
            Vector3 move = m_Motor.GetCurrentVelocity();
            var next = (move.sqrMagnitude > 0.001f) ? LocomotionState.Running : LocomotionState.Idle;
            if (next != m_Locomotion) ApplyLocomotion(next);
        }

        // 2) Independent aiming (disabled during dash)
        bool aiming = m_WaterBalloon != null && m_WaterBalloon.IsAiming && m_Locomotion != LocomotionState.Dash;
        ApplyAiming(aiming);
    }

    private void HandleShot()
    {
        // Shooting does not interrupt locomotion; only fires the trigger
        if (m_Animator != null && !string.IsNullOrEmpty(m_AnimTriggerShoot))
            m_Animator.SetTrigger(m_AnimTriggerShoot);
    }

    public void OnEvent(DashStartEvent evt)
    {
        ApplyLocomotion(LocomotionState.Dash);
        // Force aiming off during dash
        ApplyAiming(false);
    }

    public void OnEvent(DashEndEvent evt)
    {
        // Re-evaluate locomotion immediately
        Vector3 move = m_Motor.GetCurrentVelocity();
        var next = (move.sqrMagnitude > 0.001f) ? LocomotionState.Running : LocomotionState.Idle;
        ApplyLocomotion(next);

        // Restore aiming if active in the skill
        bool aiming = m_WaterBalloon != null && m_WaterBalloon.IsAiming;
        ApplyAiming(aiming);
    }

    private void ApplyLocomotion(LocomotionState newState)
    {
        if (m_Locomotion == newState) return;
        m_Locomotion = newState;

        switch (m_Locomotion)
        {
            case LocomotionState.Idle:
                if (m_Animator != null)
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                StopRunParticles();
                break;

            case LocomotionState.Running:
                if (m_Animator != null)
                    m_Animator.SetBool(m_AnimParamIsRunning, true);
                PlayRunParticles();
                break;

            case LocomotionState.Dash:
                if (m_Animator != null)
                {
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                    if (!string.IsNullOrEmpty(m_AnimTriggerDash))
                        m_Animator.SetTrigger(m_AnimTriggerDash);
                }
                StopRunParticles();
                break;
        }
    }

    private void ApplyAiming(bool aiming)
    {
        if (m_Animator != null)
            m_Animator.SetBool(m_AnimParamIsAiming, aiming);
    }

    private void PlayRunParticles()
    {
        if (m_RunParticles != null && !m_RunParticles.isPlaying)
            m_RunParticles.Play();
    }

    private void StopRunParticles()
    {
        if (m_RunParticles != null && m_RunParticles.isPlaying)
            // Stop particle emission, let existing particles fade out
            m_RunParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}