using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using MoreMountains.Feedbacks; // o MoreMountains.Tools si es el namespace de MMPool
using MoreMountains.Tools; // Namespace de MMSimpleObjectPooler

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

        // Usa el método utilitario centralizado
        List<Vector3> dotPositions = DotPlacementUtils.PoissonDiskSample(
            AreaCenter, AreaSize, MinDistance, MaxDots, MaxAttempts
        );

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AreaCenter, AreaSize);
    }
}