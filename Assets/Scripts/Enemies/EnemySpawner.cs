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
    [Tooltip("Total number of bosses that must be spawned across all waves (guaranteed, distributed randomly)")]
    public int GuaranteedBosses = 0;
    [Range(0f, 1f), Tooltip("Chance per wave to spawn ONE additional boss (does not count towards the guarantee)")]
    public float BossSpawnChance = 0.2f;

    private int m_CurrentWave = 0;
    private bool m_GameEnded = false;
    private Coroutine m_SpawnCoroutine;

    // Boss tracking
    private int m_GuaranteedBossesSpawned = 0; // counts only guaranteed bosses
    private int[] m_BossesPerWave; // planned guaranteed bosses per wave (random distribution)

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
        m_GuaranteedBossesSpawned = 0;

        BuildBossPlan();

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
    }

    private void BuildBossPlan()
    {
        if (MaxWaves <= 0 || GuaranteedBosses <= 0)
        {
            m_BossesPerWave = null;
            return;
        }

        m_BossesPerWave = new int[MaxWaves];
        // Randomly assign each guaranteed boss to a wave index [0..MaxWaves-1]
        for (int i = 0; i < GuaranteedBosses; i++)
        {
            int waveIndex = Random.Range(0, MaxWaves);
            m_BossesPerWave[waveIndex]++;
        }
    }

    private IEnumerator SpawnWaves()
    {
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
        m_SpawnCoroutine = null;
    }

    private IEnumerator SpawnWaveAsync()
    {
        if (m_GameEnded) yield break;

        // Spawn regular enemies
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
                // Additional enemy initialization if necessary
            }
            else
            {
                Debug.LogWarning($"EnemySpawner: failed to instantiate enemy. {op.OperationException}");
            }
        }

        // Spawn guaranteed bosses for this wave (according to the random plan)
        if (!m_GameEnded && IsValid(BossPrefab) && GuaranteedBosses > 0)
        {
            int plannedForThisWave = 0;
            if (m_BossesPerWave != null && m_CurrentWave >= 0 && m_CurrentWave < m_BossesPerWave.Length)
            {
                plannedForThisWave = m_BossesPerWave[m_CurrentWave];
            }

            // Ensure the guarantee is met by the last wave
            if (m_CurrentWave == MaxWaves - 1)
            {
                int remaining = Mathf.Max(0, GuaranteedBosses - m_GuaranteedBossesSpawned);
                plannedForThisWave = Mathf.Max(plannedForThisWave, remaining);
            }

            for (int i = 0; i < plannedForThisWave && !m_GameEnded; i++)
            {
                Vector3 bossSpawnPos = GetRandomPointAround(transform.position, SpawnRadius);
                AsyncOperationHandle<GameObject> op = BossPrefab.InstantiateAsync(bossSpawnPos, Quaternion.identity);
                yield return op;

                if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
                {
                    EnsureAddressableAutoRelease(op.Result);
                    m_GuaranteedBossesSpawned++;
                }
                else
                {
                    Debug.LogWarning($"EnemySpawner: failed to instantiate boss. {op.OperationException}");
                    // No increment on failure; last wave logic will try to compensate
                }
            }
        }

        // Probabilistic boss (does not count towards the guaranteed total)
        if (!m_GameEnded && IsValid(BossPrefab) && BossSpawnChance > 0f)
        {
            if (Random.value < BossSpawnChance)
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
                    Debug.LogWarning($"EnemySpawner: failed to instantiate (random) boss. {op.OperationException}");
                }
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
