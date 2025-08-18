using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class SerializedHashSet<V> : HashSet<V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<V> m_Values = new List<V>();

        public SerializedHashSet() : base()
        {
            m_Values = new List<V>();
        }

        public SerializedHashSet(HashSet<V> baseHashSet) : base(baseHashSet)
        {
            m_Values = new List<V>();
        }

        /// <summary>
        /// OnBeforeSerialize implementation.
        /// </summary>
        public void OnBeforeSerialize()
        {
            m_Values.Clear();

            foreach (V value in this)
            {
                m_Values.Add(value);
            }
        }

        /// <summary>
        /// OnAfterDeserialize implementation.
        /// </summary>
        public void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Values.Count; i++)
                this.Add(m_Values[i]);

            m_Values.Clear();
        }
    }
}
