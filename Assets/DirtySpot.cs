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
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resetea el estado al reutilizar desde el pool.
    /// Llama esto ANTES de usarla si a�n no se ha activado, o justo tras posicionarla.
    /// </summary>
    public void ResetState()
    {
        IsClean = false;
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        // Aqu� podr�as resetear materiales, tama�o, etc.
    }
}