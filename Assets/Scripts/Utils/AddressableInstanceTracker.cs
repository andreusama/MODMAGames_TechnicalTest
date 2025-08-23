using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableInstanceTracker : MonoBehaviour
{
    [HideInInspector] public bool FromAddressables;

    private bool m_Released;

    private void OnDestroy()
    {
        // Releases the instance created by Addressables to avoid leaks.
        if (!m_Released && FromAddressables)
        {
            m_Released = true;
            Addressables.ReleaseInstance(gameObject);
        }
    }
}