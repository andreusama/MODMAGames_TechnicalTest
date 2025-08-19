using UnityEngine;

public static class GroundDetector
{
    /// <summary>
    /// Devuelve la posición en el suelo bajo el punto dado, o el mismo punto si no hay suelo.
    /// </summary>
    public static Vector3 GetGroundedPosition(Vector3 point, float rayHeight = 2f, float rayDistance = 10f, LayerMask? groundMask = null)
    {
        Ray ray = new Ray(point + Vector3.up * rayHeight, Vector3.down);
        LayerMask mask = groundMask ?? ~0; // Si no se pasa máscara, colisiona con todo
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, mask))
        {
            return hit.point;
        }
        return point;
    }
}