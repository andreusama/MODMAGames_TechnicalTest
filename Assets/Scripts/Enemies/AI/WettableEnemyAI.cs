using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

public class WettableEnemyAI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField, Child] private Animator m_Animator;
    [SerializeField] private string m_AnimParamIsRunning = "IsRunning";
    [SerializeField] private string m_AnimTriggerDie = "Die";

    private enum EnemyState { Run, Die }
    private EnemyState m_State = EnemyState.Run;

    private NavMeshAgent m_Agent;

    private IWettable m_Wettable;
    private int m_LastWetness = -1;
    private float m_OriginalSpeed;

    [SerializeField, Self]
    private WettableEnemy m_WettableObject; // To listen for OnExplode

    private Transform m_PlayerTransform;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        m_Agent.avoidancePriority = 99;

        if (m_Agent != null)
            m_OriginalSpeed = m_Agent.speed;

        m_Wettable = GetComponent<IWettable>();
        if (m_WettableObject != null)
            m_WettableObject.OnExplode += HandleEnemyExploded;
    }

    private void Start()
    {
        SetState(EnemyState.Run);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            m_PlayerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        // If dead, do nothing else
        if (m_State == EnemyState.Die)
            return;

        if (m_PlayerTransform == null)
        {
            if (m_Agent != null) m_Agent.isStopped = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, m_PlayerTransform.position);

        if (m_Agent != null && m_Agent.enabled)
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(m_PlayerTransform.position);
        }
    }

    private void OnDestroy()
    {
        if (m_WettableObject != null)
            m_WettableObject.OnExplode -= HandleEnemyExploded;
    }

    public float GetOriginalSpeed()
    {
        return m_OriginalSpeed;
    }

    public void SetSpeed(float speed)
    {
        if (m_Agent != null)
            m_Agent.speed = m_OriginalSpeed * speed;
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
                if (m_Agent != null && m_Agent.enabled) m_Agent.isStopped = false;
                if (m_Animator != null) m_Animator.SetBool(m_AnimParamIsRunning, true);
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