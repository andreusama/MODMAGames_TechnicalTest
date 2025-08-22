using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

public class WettableEnemyAI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField, Child] private Animator m_Animator;
    [SerializeField] private string AnimParamIsRunning = "IsRunning";
    [SerializeField] private string AnimTriggerDie = "Die";

    private enum EnemyState { Run, Die }
    private EnemyState m_State = EnemyState.Run;

    private NavMeshAgent agent;

    private IWettable wettable;
    private int lastWetness = -1;
    private float originalSpeed;

    [SerializeField, Self]
    private WettableEnemy wettableObject; // Para escuchar OnDied

    private Transform playerTransform;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.avoidancePriority = 99;

        if (agent != null)
            originalSpeed = agent.speed;

        wettable = GetComponent<IWettable>();
        if (wettableObject != null)
            wettableObject.OnExplode += HandleEnemyExploded;
    }

    private void Start()
    {
        SetState(EnemyState.Run);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        // Si murió, no hacer nada más
        if (m_State == EnemyState.Die)
            return;

        if (playerTransform == null)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
    }

    private void OnDestroy()
    {
        if (wettableObject != null)
            wettableObject.OnExplode -= HandleEnemyExploded;
    }

    public float GetOriginalSpeed()
    {
        return originalSpeed;
    }

    public void SetSpeed(float speed)
    {
        if (agent != null)
            agent.speed = originalSpeed * speed;
    }

    private void HandleEnemyExploded(WettableEnemy e)
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