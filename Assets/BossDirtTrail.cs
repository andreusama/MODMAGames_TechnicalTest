using UnityEngine;

public class BossDirtTrail : MonoBehaviour
{
    public GameObject DirtDotPrefab;
    public float SpawnInterval = 0.5f;
    public float MinDistance = 0.5f;

    private Vector3 lastSpawnPos;
    private float timer = 0f;

    private void Start()
    {
        lastSpawnPos = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= SpawnInterval && Vector3.Distance(transform.position, lastSpawnPos) > MinDistance)
        {
            Quaternion decalRotation = Quaternion.Euler(90f, 0f, 0f);
            // Offset de 1.08 en el eje Y local del decal (hacia arriba tras la rotación)
            Instantiate(DirtDotPrefab, transform.position, decalRotation);
            lastSpawnPos = transform.position;
            timer = 0f;
        }
    }
}