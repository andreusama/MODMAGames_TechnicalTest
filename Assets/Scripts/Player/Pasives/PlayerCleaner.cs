using UnityEngine;

public class PlayerCleaner : MonoBehaviour
{
    public float CleanRadius = 0.5f;
    public LayerMask DirtySpotLayer;

    private void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, CleanRadius, DirtySpotLayer);
        foreach (var hit in hits)
        {
            var cleanable = hit.GetComponent<ICleanable>();
            if (cleanable != null && !cleanable.IsClean)
                cleanable.Clean();
        }
    }
}