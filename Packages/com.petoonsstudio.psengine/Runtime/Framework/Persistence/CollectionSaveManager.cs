using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [System.Serializable]
    public class CollectionSaveManager
    {
        public Dictionary<string, List<BehaviourPersistentData>> Collection;

        public CollectionSaveManager()
        {
            Collection = new Dictionary<string, List<BehaviourPersistentData>>();
        }

        public void NewRegister(BehaviourPersistentData data)
        {
            if (Collection.ContainsKey(data.Guid))
            {
                var list = Collection[data.Guid];

                list.RemoveAll(x => x.GetType() == data.GetType());
                list.Add(data);
            }
            else
            {
                Collection[data.Guid] = new List<BehaviourPersistentData>
                {
                    data
                };
            }
        }

        public bool TryGetRegister<T>(string guid, out T data) where T : BehaviourPersistentData
        {
            data = null;

            if (Collection.ContainsKey(guid))
            {
                var list = Collection[guid];

                data = list.First(x => x.GetType() == typeof(T)) as T;

                if (data != null)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            Collection.Clear();
        }
    }

    public interface IPersistentBehaviour<T> where T : BehaviourPersistentData
    {
        public abstract string Guid { get; }
        public void SaveBehaviour();
        public void LoadBehaviour();
        public PersistenceParams PersistenceParams { get; }
    }

    public class BehaviourPersistentData
    {
        public string Guid;

        public BehaviourPersistentData(string guid)
        {
            Guid = guid;
        }
    }
}