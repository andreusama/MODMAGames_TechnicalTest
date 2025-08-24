using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerSpawner : MonoBehaviour, IEventListener<GameStartEvent>
{
    [Header("Settings")]
    public AssetReferenceGameObject PlayerReference; // Addressable
    public Transform SpawnPoint;
    private GameObject m_PlayerInstance;

    private void OnEnable()
    {
        EventManager.AddListener<GameStartEvent>(this);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<GameStartEvent>(this);
    }

    public void OnEvent(GameStartEvent e)
    {
        if (m_PlayerInstance == null)
        {
            StartCoroutine(SpawnPlayerAsync());
        }
        else
        {
            Debug.LogWarning("Player already spawned.");
        }
    }

    private IEnumerator SpawnPlayerAsync()
    {
        if (PlayerReference == null || !PlayerReference.RuntimeKeyIsValid())
        {
            Debug.LogWarning("PlayerSpawner: PlayerReference is null or not a valid Addressable key.");
            yield break;
        }

        AsyncOperationHandle<GameObject> op = PlayerReference.InstantiateAsync(
            SpawnPoint != null ? SpawnPoint.position : Vector3.zero,
            SpawnPoint != null ? SpawnPoint.rotation : Quaternion.identity
        );

        yield return op;

        if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
        {
            m_PlayerInstance = op.Result;
            EnsureAddressableAutoRelease(m_PlayerInstance);
            Debug.Log("Player spawned at " + (SpawnPoint != null ? SpawnPoint.position : m_PlayerInstance.transform.position));

            // Wait until HUDController singleton exists
            yield return new WaitUntil(() => HUDController.Instance != null);
            
            if (m_PlayerInstance == null) yield break; ; // bail out if destroyed
            var healthComponent = m_PlayerInstance.GetComponent<PlayerHealth>();

            if (healthComponent != null && HUDController.Instance != null)
            {
                HUDController.Instance.CreateHUD(healthComponent);
            }
        }
        else
        {
            Debug.LogWarning($"PlayerSpawner: failed to instantiate player. {op.OperationException}");
        }
    }

    private static void EnsureAddressableAutoRelease(GameObject go)
    {
        var tracker = go.GetComponent<AddressableInstanceTracker>();
        if (tracker == null) tracker = go.AddComponent<AddressableInstanceTracker>();
        tracker.FromAddressables = true;
    }
}

