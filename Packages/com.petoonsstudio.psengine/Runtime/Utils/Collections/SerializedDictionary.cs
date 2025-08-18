using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<K> m_Keys = new List<K>();

        [SerializeField]
        List<V> m_Values = new List<V>();

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

        /// <summary>
        /// OnBeforeSerialize implementation.
        /// </summary>
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

        /// <summary>
        /// OnAfterDeserialize implementation.
        /// </summary>
        public void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Keys.Count; i++)
                this[m_Keys[i]] = m_Values[i];

            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}