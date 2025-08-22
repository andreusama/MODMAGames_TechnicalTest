using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// Pool h�brido:
/// - Puede adoptar objetos ya bakeados como hijos (incluso si no est�n en la lista).
/// - Puede completar hasta un tama�o objetivo instanciando el prefab si faltan.
/// - Puede expandirse din�micamente si PoolCanExpand = true.
/// Requiere el c�digo de MMSimpleObjectPooler en el proyecto (hereda de �l).
/// </summary>
public class BakedSimpleObjectPooler : MMSimpleObjectPooler
{
    [Header("Baked Injection")]
    [Tooltip("Si est� activo, adoptar� los hijos ya existentes como parte del pool.")]
    public bool UseBakedChildren = true;

    [Tooltip("Lista opcional expl�cita. Si est� vac�a y UseBakedChildren est� activo, usar� TODOS los hijos.")]
    public List<GameObject> BakedObjects = new List<GameObject>();

    [Tooltip("Asegura que el pool tenga al menos este n�mero tras inyectar (instancia si faltan). 0 = ignora.")]
    public int MinimumPoolSize = 0;

    [Tooltip("Forzar desactivar (SetActive false) a todos los bakeados al inyectarlos.")]
    public bool DeactivateOnInjection = true;

    protected bool _injected = false;

    public override void FillObjectPool()
    {
        // Crea / reutiliza waiting pool
        if (_objectPool == null)
        {
            CreateWaitingPool();
        }

        if (_objectPool.PooledGameObjects == null)
            _objectPool.PooledGameObjects = new List<GameObject>();
        else
            _objectPool.PooledGameObjects.Clear();

        // 1. Inyecta objetos bakeados
        if (UseBakedChildren)
        {
            InjectBakedChildren();
        }

        // 2. Instancia hasta PoolSize (comportamiento MMSimpleObjectPooler est�ndar)
        //    Solo si PoolSize > ya inyectados y hay prefab.
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
    /// Devuelve un objeto inactivo; si no hay y se puede expandir, crea uno.
    /// </summary>
    public override GameObject GetPooledGameObject()
    {
        // Busca libre
        for (int i = 0; i < _objectPool.PooledGameObjects.Count; i++)
        {
            var obj = _objectPool.PooledGameObjects[i];
            if (obj != null && !obj.activeInHierarchy)
                return obj;
        }

        // Expansi�n
        if (PoolCanExpand && GameObjectToPool != null)
        {
            return CreateOne(true);
        }

        return null;
    }

    protected void InjectBakedChildren()
    {
        if (_injected) return;

        List<GameObject> source = BakedObjects.Count > 0 ? BakedObjects : CollectAllChildren();
        foreach (var go in source)
        {
            if (go == null) continue;
            if (DeactivateOnInjection)
                go.SetActive(false);
            go.transform.SetParent(_waitingPool.transform);
            _objectPool.PooledGameObjects.Add(go);
        }
        _injected = true;
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
    /// Crea una instancia del prefab y la a�ade al pool.
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