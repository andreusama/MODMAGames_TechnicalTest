using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject EnemyPrefab;
    public int EnemiesPerWave = 5;
    public float SpawnRadius = 10f;
    public float TimeBetweenWaves = 10f;
    public int MaxWaves = 10;

    private int m_CurrentWave = 0;
    private bool m_Spawning = false;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        m_Spawning = true;
        while (m_CurrentWave < MaxWaves)
        {
            SpawnWave();
            m_CurrentWave++;
            yield return new WaitForSeconds(TimeBetweenWaves);
        }
        m_Spawning = false;
    }

    private void SpawnWave()
    {
        for (int i = 0; i < EnemiesPerWave; i++)
        {
            Vector3 spawnPos = GetRandomPointAround(transform.position, SpawnRadius);
            GameObject enemy = Instantiate(EnemyPrefab, spawnPos, Quaternion.identity);

            // Opcional: puedes inicializar aquí cualquier lógica extra del enemigo
            // Por ejemplo, si tu EnemyAI necesita alguna referencia especial, puedes pasarla aquí
        }
    }

    private Vector3 GetRandomPointAround(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0.5f * radius, radius);
        Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        // Ajusta la altura al terreno si es necesario
        pos.y = center.y;
        return pos;
    }
}
