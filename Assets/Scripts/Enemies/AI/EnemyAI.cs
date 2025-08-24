using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private EnemyBaseConfig m_Config;

    public float Damage;
    public float AttackRange;
    public float AttackCooldown;

    [Header("Animation")]
    [SerializeField, Child] private Animator m_Animator;
    [SerializeField] private string m_AnimParamIsRunning = "IsRunning";
    [SerializeField] private string m_AnimTriggerAttack = "Attack";
    [SerializeField] private string m_AnimTriggerDie = "Die";

    private enum EnemyState { Run, Attack, Die }
    private EnemyState m_State = EnemyState.Run;

    private NavMeshAgent m_Agent;
    private PlayerHealth m_PlayerHealth;
    private Transform m_PlayerTransform;
    private float m_LastAttackTime = -999f;

    private float m_OriginalSpeed;

    [SerializeField, Self]
    private Enemy m_Enemy; // To listen for OnDied

    private void Awake()
    {
        if (m_Config == null)
        {
            Debug.LogWarning($"{name}: EnemyBaseConfig not assigned. Disabling EnemyAI.");
            enabled = false;
            return;
        }

        Damage = m_Config.Damage;
        AttackRange = m_Config.AttackRange;
        AttackCooldown = m_Config.AttackCooldown;

        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        m_Agent.avoidancePriority = 99;

        if (m_Agent != null)
            m_OriginalSpeed = m_Agent.speed;

        if (m_Enemy != null)
            m_Enemy.OnDied += HandleEnemyDied;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            m_PlayerTransform = playerObj.transform;
            m_PlayerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        // Initial state
        SetState(EnemyState.Run);
    }

    private void OnDestroy()
    {
        if (m_Enemy != null)
            m_Enemy.OnDied -= HandleEnemyDied;
    }

    private void Update()
    {
        // If dead, do nothing else
        if (m_State == EnemyState.Die)
            return;

        if (m_PlayerTransform == null || m_PlayerHealth == null || !m_PlayerHealth.IsAlive)
        {
            if (m_Agent != null) m_Agent.isStopped = true;
            if (m_Animator != null) m_Animator.SetBool(m_AnimParamIsRunning, false);
            return;
        }

        float distance = Vector3.Distance(transform.position, m_PlayerTransform.position);

        if (distance <= AttackRange)
        {
            SetState(EnemyState.Attack);

            // Face the player
            Vector3 dir = m_PlayerTransform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

            // Apply damage on cooldown (you can move this to an animation event to sync the impact)
            if (Time.time >= m_LastAttackTime + AttackCooldown)
            {
                m_PlayerHealth.TakeDamage(Damage);
                m_LastAttackTime = Time.time;

                // Trigger attack (if not using animation event, damage already applied above)
                if (m_Animator != null && !string.IsNullOrEmpty(m_AnimTriggerAttack))
                    m_Animator.SetTrigger(m_AnimTriggerAttack);
            }
        }
        else
        {
            SetState(EnemyState.Run);
            if (m_Agent != null && m_Agent.enabled)
            {
                m_Agent.isStopped = false;
                m_Agent.SetDestination(m_PlayerTransform.position);
            }
        }
    }

    public float GetOriginalSpeed()
    {
        return m_OriginalSpeed;
    }

    public void SetSpeed(float speed)
    {
        if (m_Agent != null)
            m_Agent.speed = speed;
    }

    private void HandleEnemyDied(Enemy e)
    {
        SetState(EnemyState.Die);
    }

    private void SetState(EnemyState newState)
    {
        if (m_State == newState) return;
        m_State = newState;

        switch (m_State)
        {
            case EnemyState.Run:
                if (m_Agent != null && m_Agent.enabled) m_Agent.isStopped = false;
                if (m_Animator != null) m_Animator.SetBool(m_AnimParamIsRunning, true);
                break;

            case EnemyState.Attack:
                if (m_Agent != null) m_Agent.isStopped = true;
                if (m_Animator != null) m_Animator.SetBool(m_AnimParamIsRunning, false);
                break;

            case EnemyState.Die:
                if (m_Agent != null)
                {
                    m_Agent.isStopped = true;
                    m_Agent.ResetPath();
                    m_Agent.enabled = false;
                }
                if (m_Animator != null)
                {
                    m_Animator.SetBool(m_AnimParamIsRunning, false);
                    if (!string.IsNullOrEmpty(m_AnimTriggerDie))
                        m_Animator.SetTrigger(m_AnimTriggerDie);
                }
                break;
        }
    }
}