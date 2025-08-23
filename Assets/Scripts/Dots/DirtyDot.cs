using UnityEngine;

public class DirtyDot : MonoBehaviour, ICleanable
{
    public bool IsClean { get; private set; } = false;

    private void OnEnable()
    {
        DirtyDotManager.Instance?.RegisterDot(this);
    }

    private void OnDisable()
    {
        DirtyDotManager.Instance?.UnregisterDot(this);
    }

    public void Clean()
    {
        if (IsClean) return;
        IsClean = true;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets state when reused from pool.
    /// Call this BEFORE using it if it hasn't been activated yet, or right after positioning it.
    /// </summary>
    public void ResetState()
    {
        IsClean = false;
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        // You could reset materials, size, etc. here if needed.
    }
}