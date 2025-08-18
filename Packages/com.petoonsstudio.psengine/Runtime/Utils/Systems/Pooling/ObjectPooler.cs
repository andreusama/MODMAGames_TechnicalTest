using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private GameObject m_PooledItem;
        [SerializeField] private int m_PoolAmount;
        [SerializeField] private bool m_ParentTransform;

        private List<GameObject> m_PooledObjects;
        private Transform m_PoolContainer;
        public Transform Container { get => m_PoolContainer; }

        protected virtual void Awake()
        {
            m_PooledObjects = new List<GameObject>();

            CreatePoolContainer();

            for (int i = 0; i < m_PoolAmount; i++)
            {
                CreateNewPooledInstance();
            }
        }
        public virtual GameObject GetPooledObject()
        {
            foreach (var pooledItem in m_PooledObjects)
            {
                if (!pooledItem.activeInHierarchy)
                {
                    return pooledItem;
                }
            }

            var newItem = CreateNewPooledInstance();
            return newItem;
        }

        public virtual void SleepAll()
        {
            foreach (var pooledItem in m_PooledObjects)
            {
                pooledItem.SetActive(false);
            }
        }
        protected virtual GameObject CreateNewPooledInstance()
        {
            GameObject tmp = Instantiate(m_PooledItem, m_PoolContainer);
            tmp.SetActive(false);

            m_PooledObjects.Add(tmp);

            return tmp;
        }

        protected virtual void CreatePoolContainer()
        {
            m_PoolContainer = new GameObject($"{m_PooledItem.name} Pool").transform;

            if (m_ParentTransform)
            {
                m_PoolContainer.SetParent(transform);
            }
        }
    }
}

