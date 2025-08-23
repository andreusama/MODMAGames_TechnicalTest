using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// Hybrid pool:
/// - Can adopt already baked children as pool members (even if not listed).
/// - Can fill up to a target size instantiating the prefab if missing.
/// - Can expand dynamically if PoolCanExpand = true.
/// Requires MMSimpleObjectPooler code in the project (inherits from it).
/// </summary>
public class BakedSimpleObjectPooler : MMSimpleObjectPooler
{
    [Header("Baked Injection")]
    [Tooltip("If active, it will adopt existing children as part of the pool.")]
    public bool UseBakedChildren = true;

    [Tooltip("Optional explicit list. If empty and UseBakedChildren is active, will use ALL children.")]
    public List<GameObject> BakedObjects = new List<GameObject>();

    [Tooltip("Ensures the pool has at least this number after injection (instantiates if missing). 0 = ignore.")]
    public int MinimumPoolSize = 0;

    [Tooltip("Force deactivate (SetActive false) all baked objects when injecting.")]
    public bool DeactivateOnInjection = true;

    protected bool m_Injected = false;

    public override void FillObjectPool()
    {
        // Create / reuse waiting pool
        if (_objectPool == null)
        {
            CreateWaitingPool();
        }

        if (_objectPool.PooledGameObjects == null)
            _objectPool.PooledGameObjects = new List<GameObject>();
        else
            _objectPool.PooledGameObjects.Clear();

        // 1. Inject baked children
        if (UseBakedChildren)
        {
            InjectBakedChildren();
        }

        // 2. Instantiate up to PoolSize (standard MMSimpleObjectPooler behavior)
        //    Only if PoolSize > injected and prefab exists.
        int target = PoolSize;
        if (MinimumPoolSize > target)
            target = MinimumPoolSize;

        int current = _objectPool.PooledGameObjects.Count;

        if (GameObjectToPool != null && current < target)
        {
            int toCreate = target - current;
            for (int i = 0; i < toCreate; i++)
            {
                CreateOne(false);
            }
        }
    }

    /// <summary>
    /// Returns an inactive object; if none and expansion allowed, creates one.
    /// </summary>
    public override GameObject GetPooledGameObject()
    {
        // Find free
        for (int i = 0; i < _objectPool.PooledGameObjects.Count; i++)
        {
            var obj = _objectPool.PooledGameObjects[i];
            if (obj != null && !obj.activeInHierarchy)
                return obj;
        }

        // Expand
        if (PoolCanExpand && GameObjectToPool != null)
        {
            return CreateOne(true);
        }

        return null;
    }

    protected void InjectBakedChildren()
    {
        if (m_Injected) return;

        List<GameObject> source = BakedObjects.Count > 0 ? BakedObjects : CollectAllChildren();
        foreach (var go in source)
        {
            if (go == null) continue;
            if (DeactivateOnInjection)
                go.SetActive(false);
            go.transform.SetParent(_waitingPool.transform);
            _objectPool.PooledGameObjects.Add(go);
        }
        m_Injected = true;
    }

    protected List<GameObject> CollectAllChildren()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            list.Add(transform.GetChild(i).gameObject);
        }
        return list;
    }

    /// <summary>
    /// Creates an instance of the prefab and adds it to the pool.
    /// </summary>
    protected GameObject CreateOne(bool expanding)
    {
        GameObject newObj = Instantiate(GameObjectToPool);
        newObj.name = expanding ? $"{GameObjectToPool.name}_Extra" : GameObjectToPool.name;
        newObj.transform.SetParent(_waitingPool.transform);
        newObj.SetActive(false);
        _objectPool.PooledGameObjects.Add(newObj);
        return newObj;
    }
}