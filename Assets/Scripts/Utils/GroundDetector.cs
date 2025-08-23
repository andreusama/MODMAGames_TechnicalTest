using UnityEngine;

public static class GroundDetector
{
    /// <summary>
    /// Returns the ground position below the given point, or the same point if no ground is found.
    /// </summary>
    public static Vector3 GetGroundedPosition(Vector3 point, float rayHeight = 2f, float rayDistance = 10f, LayerMask? groundMask = null)
    {
        Ray ray = new Ray(point + Vector3.up * rayHeight, Vector3.down);
        LayerMask mask = groundMask ?? ~0; // If no mask is provided, collide with everything
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, mask))
        {
            return hit.point;
        }
        return point;
    }
}