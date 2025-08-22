using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float Damage = 10f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;

    [Header("Wettable Slowdown")]
    [Range(0.1f, 1f)]
    public float MinSpeedPercent = 0.3f; // 30% de la velocidad original cuando está completamente mojado

    [Header("Animation")]
    [SerializeField, Child] private Animator m_Animator;
    [SerializeField] private string AnimParamIsRunning = "IsRunning";
    [SerializeField] private string AnimTriggerAttack = "Attack";
    [SerializeField] private string AnimTriggerDie = "Die";

    private enum EnemyState { Run, Attack, Die }
    private EnemyState m_State = EnemyState.Run;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private Transform playerTransform;
    private float lastAttackTime = -999f;

    private IWettable wettable;
    private int lastWetness = -1;
    private float originalSpeed;

    [SerializeField, Self]
    private Enemy enemy; // Para escuchar OnDied

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.avoidancePriority = 99;

        wettable = GetComponent<IWettable>();
        if (agent != null)
            originalSpeed = agent.speed;

        
        if (enemy != null)
            enemy.OnDied += HandleEnemyDied;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        // Estado inicial
        SetState(EnemyState.Run);
    }

    private void OnDestroy()
    {
        if (enemy != null)
            enemy.OnDied -= HandleEnemyDied;
    }

    private void Update()
    {
        // Si murió, no hacer nada más
        if (m_State == EnemyState.Die)
            return;

        if (playerTransform == null || playerHealth == null || !playerHealth.IsAlive)
        {
            if (agent != null) agent.isStopped = true;
            if (m_Animator != null) m_Animator.SetBool(AnimParamIsRunning, false);
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= AttackRange)
        {
            SetState(EnemyState.Attack);

            // Orienta hacia el jugador
            Vector3 dir = playerTransform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

            // Aplica daño por cooldown (puedes mover esto a un evento de animación si prefieres sincronizar el impacto)
            if (Time.time >= lastAttackTime + AttackCooldown)
            {
                playerHealth.TakeDamage(Damage);
                lastAttackTime = Time.time;

                // Dispara trigger de ataque (si no usas evento de anim, el daño ya se aplicó arriba)
                if (m_Animator != null && !string.IsNullOrEmpty(AnimTriggerAttack))
                    m_Animator.SetTrigger(AnimTriggerAttack);
            }
        }
        else
        {
            SetState(EnemyState.Run);
            if (agent != null && agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
        }
    }

    public float GetOriginalSpeed()
    {
        return originalSpeed;
    }

    public void SetSpeed(float speed)
    {
        if (agent != null)
            agent.speed = speed;
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
                if (agent != null && agent.enabled) agent.isStopped = false;
                if (m_Animator != null) m_Animator.SetBool(AnimParamIsRunning, true);
                break;

            case EnemyState.Attack:
                if (agent != null) agent.isStopped = true;
                if (m_Animator != null) m_Animator.SetBool(AnimParamIsRunning, false);
                break;

            case EnemyState.Die:
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.enabled = false;
                }
                if (m_Animator != null)
                {
                    m_Animator.SetBool(AnimParamIsRunning, false);
                    if (!string.IsNullOrEmpty(AnimTriggerDie))
                        m_Animator.SetTrigger(AnimTriggerDie);
                }
                break;
        }
    }
}