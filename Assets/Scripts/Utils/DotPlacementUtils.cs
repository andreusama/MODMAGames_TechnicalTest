using System.Collections.Generic;
using UnityEngine;

public static class DotPlacementUtils
{
    /// <summary>
    /// Distribución simple estilo Poisson (rechazo) en rectángulo (size.x ancho, size.z profundidad).
    /// </summary>
    public static List<Vector3> PoissonDiskSample(
        Vector3 center,
        Vector3 size,
        float minDist,
        int maxCount,
        int maxAttempts = 10000)
    {
        List<Vector3> points = new List<Vector3>();
        int attempts = 0;
        while (points.Count < maxCount && attempts < maxAttempts)
        {
            float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
            float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);
            Vector3 candidate = new Vector3(x, center.y, z);

            if (IsFarEnough(points, candidate, minDist))
            {
                points.Add(candidate);
            }
            attempts++;
        }
        return points;
    }

    /// <summary>
    /// Distribución simple estilo Poisson (rechazo) en círculo (radio en el plano XZ).
    /// </summary>
    public static List<Vector3> PoissonDiskSampleCircle(
        Vector3 center,
        float radius,
        float minDist,
        int maxCount,
        int maxAttempts = 10000)
    {
        List<Vector3> points = new List<Vector3>();
        int attempts = 0;
        while (points.Count < maxCount && attempts < maxAttempts)
        {
            // Generación uniforme en disco: r = sqrt(u) * R
            float u = Random.value;
            float r = Mathf.Sqrt(u) * radius;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float x = center.x + Mathf.Cos(angle) * r;
            float z = center.z + Mathf.Sin(angle) * r;
            Vector3 candidate = new Vector3(x, center.y, z);

            if (IsFarEnough(points, candidate, minDist))
            {
                points.Add(candidate);
            }
            attempts++;
        }
        return points;
    }

    private static bool IsFarEnough(List<Vector3> points, Vector3 candidate, float minDist)
    {
        float sqrMin = minDist * minDist;
        for (int i = 0; i < points.Count; i++)
        {
            if ((points[i] - candidate).sqrMagnitude < sqrMin)
                return false;
        }
        return true;
    }
}