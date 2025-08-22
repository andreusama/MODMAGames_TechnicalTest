using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableInstanceTracker : MonoBehaviour
{
    [HideInInspector] public bool FromAddressables;

    private bool _released;

    private void OnDestroy()
    {
        // Libera la instancia creada por Addressables para evitar pérdidas de referencia.
        if (!_released && FromAddressables)
        {
            _released = true;
            Addressables.ReleaseInstance(gameObject);
        }
    }
}