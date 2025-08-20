using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float Damage = 10f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;

    [Header("Wettable Slowdown")]
    [Range(0.1f, 1f)]
    public float MinSpeedPercent = 0.3f; // 30% de la velocidad original cuando est� completamente mojado

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private Transform playerTransform;
    private float lastAttackTime = -999f;

    private IWettable wettable;
    private int lastWetness = -1;
    private float originalSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.avoidancePriority = 99;

        wettable = GetComponent<IWettable>();
        if (agent != null)
            originalSpeed = agent.speed;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerTransform == null || playerHealth == null || !playerHealth.IsAlive)
            return;

        agent.SetDestination(playerTransform.position);

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= AttackRange && Time.time >= lastAttackTime + AttackCooldown)
        {
            playerHealth.TakeDamage(Damage);
            lastAttackTime = Time.time;
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
}