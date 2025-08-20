using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class DotSpawner : MonoBehaviour
{
    public GameObject DotPrefab;
    public int MaxDots = 200;
    public float MinDistance = 0.5f;
    public int MaxAttempts = 10000;
    public LayerMask LayoutLayer;
    public float RaycastHeight = 10f;
    public Vector3 AreaCenter;
    public Vector3 AreaSize;

#if UNITY_EDITOR
    [ContextMenu("Bake Dots")]
    public void BakeDots()
    {
        float dotOffsetY = 0.02f; // Offset vertical para evitar z-fighting

        // Elimina dots previos
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        List<Vector3> dotPositions = PoissonDiskSample(AreaCenter, AreaSize, MinDistance, MaxDots);

        foreach (var pos in dotPositions)
        {
            Vector3 rayOrigin = new Vector3(pos.x, AreaCenter.y + RaycastHeight, pos.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, RaycastHeight * 2, LayoutLayer))
            {
                var dot = (GameObject)PrefabUtility.InstantiatePrefab(DotPrefab, transform);
                dot.transform.position = hit.point + Vector3.up * dotOffsetY;
            }
        }
    }
#endif

    private List<Vector3> PoissonDiskSample(Vector3 center, Vector3 size, float minDist, int maxCount)
    {
        List<Vector3> points = new List<Vector3>();
        int attempts = 0;
        while (points.Count < maxCount && attempts < MaxAttempts)
        {
            float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
            Vector3 candidate = new Vector3(x, center.y, z);

            bool tooClose = false;
            foreach (var p in points)
            {
                if ((p - candidate).sqrMagnitude < minDist * minDist)
                {
                    tooClose = true;
                    break;
                }
            }
            if (!tooClose)
                points.Add(candidate);

            attempts++;
        }
        return points;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AreaCenter, AreaSize);
    }
}