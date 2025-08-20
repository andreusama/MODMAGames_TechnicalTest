using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour, IEventListener<GameStartEvent>, IEventListener<GameEndEvent>
{
    [Header("Spawner Settings")]
    public GameObject EnemyPrefab;
    public int EnemiesPerWave = 5;
    public float SpawnRadius = 10f;
    public float TimeBetweenWaves = 10f;
    public int MaxWaves = 10;

    [Header("Boss Settings")]
    public GameObject BossPrefab;
    [Range(0f, 1f)]
    public float BossSpawnChance = 0.2f; // 20% de probabilidad por oleada

    private int m_CurrentWave = 0;
    private bool m_Spawning = false;
    private bool m_GameEnded = false;
    private Coroutine m_SpawnCoroutine;

    private void OnEnable()
    {
        this.EventStartListening<GameStartEvent>();
        this.EventStartListening<GameEndEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<GameStartEvent>();
        this.EventStopListening<GameEndEvent>();
    }

    public void OnEvent(GameStartEvent e)
    {
        m_CurrentWave = 0;
        m_GameEnded = false;
        if (m_SpawnCoroutine == null)
            m_SpawnCoroutine = StartCoroutine(SpawnWaves());
    }

    public void OnEvent(GameEndEvent e)
    {
        m_GameEnded = true;
        if (m_SpawnCoroutine != null)
        {
            StopCoroutine(m_SpawnCoroutine);
            m_SpawnCoroutine = null;
        }
        m_Spawning = false;
    }

    private IEnumerator SpawnWaves()
    {
        m_Spawning = true;
        while (m_CurrentWave < MaxWaves && !m_GameEnded)
        {
            SpawnWave();
            m_CurrentWave++;
            float timer = 0f;
            while (timer < TimeBetweenWaves && !m_GameEnded)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        m_Spawning = false;
        m_SpawnCoroutine = null;
    }

    private void SpawnWave()
    {
        if (m_GameEnded) return;
        for (int i = 0; i < EnemiesPerWave; i++)
        {
            Vector3 spawnPos = GetRandomPointAround(transform.position, SpawnRadius);
            GameObject enemy = Instantiate(EnemyPrefab, spawnPos, Quaternion.identity);
            // Lógica extra de inicialización del enemigo aquí si es necesario
        }

        // Spawn boss at random on certain waves
        if (BossPrefab != null && Random.value < BossSpawnChance)
        {
            Vector3 bossSpawnPos = GetRandomPointAround(transform.position, SpawnRadius);
            Instantiate(BossPrefab, bossSpawnPos, Quaternion.identity);
        }
    }

    private Vector3 GetRandomPointAround(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0.5f * radius, radius);
        Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        pos.y = center.y;
        return pos;
    }
}
