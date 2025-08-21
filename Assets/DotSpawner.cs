using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class DotSpawner : MonoBehaviour
{
    [Header("Dot Prefab")]
    public GameObject DotPrefab;

    [Header("Distribution")]
    public int MaxDots = 200;
    public float MinDistance = 0.5f;
    public int MaxAttempts = 10000;

    [Header("Common Area Settings")]
    public Vector3 AreaCenter;
    public float RaycastHeight = 10f;
    public LayerMask LayoutLayer;

    [Header("Rectangle Area")]
    public Vector3 AreaSize = new Vector3(10, 0, 10);

    [Header("Circle Area")]
    public bool UseCircleArea = false;
    public float CircleRadius = 5f;

    [Header("Bake Visual Settings")]
    public float DotYOffset = 0.02f;

#if UNITY_EDITOR
    [ContextMenu("Bake Dots")]
    public void BakeDots()
    {
        if (DotPrefab == null)
        {
            Debug.LogWarning("DotPrefab no asignado.");
            return;
        }

        // Elimina dots previos (solo editor)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        List<Vector3> positions;

        if (UseCircleArea)
        {
            positions = DotPlacementUtils.PoissonDiskSampleCircle(
                AreaCenter, CircleRadius, MinDistance, MaxDots, MaxAttempts);
        }
        else
        {
            positions = DotPlacementUtils.PoissonDiskSample(
                AreaCenter, AreaSize, MinDistance, MaxDots, MaxAttempts);
        }

        int spawned = 0;
        foreach (var pos in positions)
        {
            Vector3 rayOrigin = new Vector3(pos.x, AreaCenter.y + RaycastHeight, pos.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, RaycastHeight * 2f, LayoutLayer))
            {
                var dot = (GameObject)PrefabUtility.InstantiatePrefab(DotPrefab, transform);
                dot.transform.position = hit.point + Vector3.up * DotYOffset;
                spawned++;
            }
        }

        Debug.Log($"DotSpawner: Bake completado. Generados {spawned} dots ({(UseCircleArea ? "círculo" : "rectángulo")}).");
    }
#endif

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (UseCircleArea)
        {
#if UNITY_EDITOR
            // Dibuja disco en el plano (solo editor con Handles)
            Handles.color = new Color(1f, 1f, 0f, 0.25f);
            Handles.DrawSolidDisc(AreaCenter, Vector3.up, CircleRadius);
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(AreaCenter, Vector3.up, CircleRadius);
#else
            // Fallback simple
            Gizmos.DrawWireSphere(AreaCenter, CircleRadius);
#endif
        }
        else
        {
            Gizmos.DrawWireCube(AreaCenter, AreaSize);
        }
    }
}