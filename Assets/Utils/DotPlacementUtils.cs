using System.Collections.Generic;
using UnityEngine;

public static class DotPlacementUtils
{
    /// <summary>
    /// Genera una lista de posiciones distribuidas usando Poisson Disk Sampling en un �rea rectangular.
    /// </summary>
    /// <param name="center">Centro del �rea.</param>
    /// <param name="size">Tama�o del �rea (Vector3.x = ancho, Vector3.z = profundidad).</param>
    /// <param name="minDist">Distancia m�nima entre puntos.</param>
    /// <param name="maxCount">M�ximo n�mero de puntos.</param>
    /// <param name="maxAttempts">Intentos m�ximos para encontrar puntos v�lidos.</param>
    public static List<Vector3> PoissonDiskSample(Vector3 center, Vector3 size, float minDist, int maxCount, int maxAttempts = 10000)
    {
        List<Vector3> points = new List<Vector3>();
        int attempts = 0;
        while (points.Count < maxCount && attempts < maxAttempts)
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
}