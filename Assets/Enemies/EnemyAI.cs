using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public float Damage = 10f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private Transform playerTransform;
    private float lastAttackTime = -999f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
}