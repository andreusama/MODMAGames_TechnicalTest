using UnityEngine;

public class DirtySpot : MonoBehaviour, ICleanable
{
    public bool IsClean { get; private set; } = false;

    private void OnEnable()
    {
        DotManager.Instance?.RegisterDot(this);
    }

    private void OnDisable()
    {
        DotManager.Instance?.UnregisterDot(this);
    }

    public void Clean()
    {
        if (IsClean) return;
        IsClean = true;
        gameObject.SetActive(false); // Esto llamará a OnDisable y desregistrará el dot
    }
}