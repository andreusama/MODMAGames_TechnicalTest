using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtils
{
    [Serializable]
    public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<K> m_Keys = new List<K>();

        [SerializeField]
        private List<V> m_Values = new List<V>();

        public SerializedDictionary() : base()
        {
            m_Keys = new List<K>();
            m_Values = new List<V>();
        }

        public SerializedDictionary(Dictionary<K, V> baseDictionary) : base(baseDictionary)
        {
            m_Keys = new List<K>();
            m_Values = new List<V>();
        }

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            foreach (var kvp in this)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Keys.Count; i++)
                this[m_Keys[i]] = m_Values[i];

            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}