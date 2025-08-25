using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class SpongeSpawner : MonoBehaviour
{
    [Header("Sponge")]
    [Tooltip("Addressable prefab of the Sponge to spawn.")]
    public AssetReferenceGameObject SpongePrefab;

    [Tooltip("Spawn automatically on Start().")]
    public bool SpawnOnStart = true;

    [Tooltip("Optional parent for the spawned Sponge.")]
    public Transform Parent;

    [Header("Spawn Area (Circle)")]
    [Tooltip("Center of the spawn area. If null, this GameObject's position is used.")]
    public Transform Center;

    [Min(0f)]
    [Tooltip("Radius of the spawn area in world units.")]
    public float Radius = 10f;

    [Range(0f, 1f)]
    [Tooltip("Percentage of radius excluded from the center (0 = no exclusion, 0.2 = exclude inner 20% of the circle).")]
    public float CenterExclusionPercent = 0.15f;

    [Min(0f)]
    [Tooltip("Radial bias exponent k. k=1 gives uniform area distribution; k>1 favors edge (harder to spawn at center).")]
    public float RadialBiasK = 3f;

    [Header("Grounding")]
    [Tooltip("Layers considered valid ground for placement.")]
    public LayerMask GroundLayers = ~0;

    [Tooltip("Vertical offset applied after grounding to avoid z-fighting.")]
    public float GroundYOffset = 0.02f;

    [Tooltip("Upwards ray start height for grounding (relative to target point).")]
    public float RayHeight = 2f;

    [Tooltip("Max raycast distance downward for grounding.")]
    public float RayDistance = 10f;

    private GameObject m_SpawnedInstance;

    private void Start()
    {
        if (SpawnOnStart)
            StartCoroutine(SpawnSpongeAsync());
    }

    [ContextMenu("Spawn Now")]
    public void Spawn()
    {
        StartCoroutine(SpawnSpongeAsync());
    }

    private IEnumerator SpawnSpongeAsync()
    {
        if (SpongePrefab == null || !SpongePrefab.RuntimeKeyIsValid())
        {
            Debug.LogWarning($"{name}: SpongePrefab is null or not a valid Addressable key.");
            yield break;
        }

        // 1) Random biased point in the circle
        Vector3 center = (Center != null) ? Center.position : transform.position;
        Vector3 randomPoint = GetBiasedRandomPointInCircle(center, Radius, CenterExclusionPercent, RadialBiasK);

        // 2) Ground it using the shared GroundDetector
        Vector3 grounded = GroundDetector.GetGroundedPosition(randomPoint, RayHeight, RayDistance, GroundLayers);
        grounded.y += GroundYOffset;

        // 3) Instantiate addressable at grounded position
        AsyncOperationHandle<GameObject> op = SpongePrefab.InstantiateAsync(grounded, Quaternion.identity, Parent);
        yield return op;

        if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
        {
            m_SpawnedInstance = op.Result;
            EnsureAddressableAutoRelease(m_SpawnedInstance);

            // 4) Signal gate
            if (GameController.Instance != null)
                GameController.Instance.SpongeSpawned = true;
        }
        else
        {
            Debug.LogWarning($"SpongeSpawner: failed to instantiate sponge. {op.OperationException}");
        }
    }

    private static void EnsureAddressableAutoRelease(GameObject go)
    {
        var tracker = go.GetComponent<AddressableInstanceTracker>();
        if (tracker == null) tracker = go.AddComponent<AddressableInstanceTracker>();
        tracker.FromAddressables = true;
    }

    /// <summary>
    /// Returns a random point in XZ plane within a circle [rMin..R] with radial density f(r) ∝ r^k.
    /// - k=1 -> uniform by area.
    /// - k>1 -> favors outer ring (harder to spawn near center).
    /// - rMin = CenterExclusionPercent * R.
    /// </summary>
    private static Vector3 GetBiasedRandomPointInCircle(Vector3 center, float radius, float exclusionPercent, float k)
    {
        float rMin = Mathf.Clamp01(exclusionPercent) * Mathf.Max(0f, radius);
        float R = Mathf.Max(rMin, radius);

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float u = Random.value;

        // Inverse CDF for f(r) ∝ r^k in [rMin, R]
        float kp1 = k + 1f;
        float a = Mathf.Pow(R, kp1);
        float b = Mathf.Pow(rMin, kp1);
        float r = Mathf.Pow(u * (a - b) + b, 1f / kp1);

        Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        return center + dir * r;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 center = (Center != null) ? Center.position : transform.position;

        // Outer circle
        UnityEditor.Handles.color = new Color(0f, 0.7f, 1f, 0.2f);
        UnityEditor.Handles.DrawSolidDisc(center, Vector3.up, Radius);
        UnityEditor.Handles.color = new Color(0f, 0.6f, 1f, 1f);
        UnityEditor.Handles.DrawWireDisc(center, Vector3.up, Radius);

        // Inner exclusion
        float rMin = Mathf.Clamp01(CenterExclusionPercent) * Mathf.Max(0f, Radius);
        if (rMin > 0.001f)
        {
            UnityEditor.Handles.color = new Color(1f, 0.3f, 0f, 0.15f);
            UnityEditor.Handles.DrawSolidDisc(center, Vector3.up, rMin);
            UnityEditor.Handles.color = new Color(1f, 0.3f, 0f, 1f);
            UnityEditor.Handles.DrawWireDisc(center, Vector3.up, rMin);
        }
    }
#endif
}
