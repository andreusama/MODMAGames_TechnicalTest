using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemySpawner : MonoBehaviour, IEventListener<GameStartEvent>, IEventListener<GameEndEvent>
{
    [Header("Spawner Settings")]
    public AssetReferenceGameObject EnemyPrefab; // Addressable
    public int EnemiesPerWave = 5;
    public float SpawnRadius = 10f;
    public float TimeBetweenWaves = 10f;
    public int MaxWaves = 10;

    [Header("Boss Settings")]
    public AssetReferenceGameObject BossPrefab; // Addressable
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
            yield return SpawnWaveAsync();
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

    private IEnumerator SpawnWaveAsync()
    {
        if (m_GameEnded) yield break;

        // Enemigos normales
        for (int i = 0; i < EnemiesPerWave && !m_GameEnded; i++)
        {
            if (!IsValid(EnemyPrefab))
                yield break;

            Vector3 spawnPos = GetRandomPointAround(transform.position, SpawnRadius);

            AsyncOperationHandle<GameObject> op = EnemyPrefab.InstantiateAsync(spawnPos, Quaternion.identity);
            yield return op;

            if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
            {
                var go = op.Result;
                EnsureAddressableAutoRelease(go);
                // Inicialización extra del enemigo si fuese necesario
            }
            else
            {
                Debug.LogWarning($"EnemySpawner: fallo al instanciar enemigo. {op.OperationException}");
            }
        }

        // Boss con probabilidad
        if (!m_GameEnded && IsValid(BossPrefab) && Random.value < BossSpawnChance)
        {
            Vector3 bossSpawnPos = GetRandomPointAround(transform.position, SpawnRadius);
            AsyncOperationHandle<GameObject> op = BossPrefab.InstantiateAsync(bossSpawnPos, Quaternion.identity);
            yield return op;

            if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
            {
                EnsureAddressableAutoRelease(op.Result);
            }
            else
            {
                Debug.LogWarning($"EnemySpawner: fallo al instanciar boss. {op.OperationException}");
            }
        }
    }

    private static bool IsValid(AssetReferenceGameObject aref)
        => aref != null && aref.RuntimeKeyIsValid();

    private static void EnsureAddressableAutoRelease(GameObject go)
    {
        var tracker = go.GetComponent<AddressableInstanceTracker>();
        if (tracker == null) tracker = go.AddComponent<AddressableInstanceTracker>();
        tracker.FromAddressables = true;
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
