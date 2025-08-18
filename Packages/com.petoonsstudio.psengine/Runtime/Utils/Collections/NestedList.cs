using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Class used for serialize lists inside other lists
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class NestedList<TValue>
    {
        [SerializeField]
        protected List<TValue> m_InnerLIst = new List<TValue>();
        public List<TValue> InnerList { get => m_InnerLIst; }

        public int Count => m_InnerLIst.Count;

        public TValue this[int key]
        {
            get
            {
                return m_InnerLIst[key];
            }
            set
            {
                m_InnerLIst[key] = value;
            }
        }

        public void Add(TValue value)
        {
            m_InnerLIst.Add(value);
        }

        public void Remove(TValue value)
        {
            m_InnerLIst.Remove(value);
        }

        public int IndexOf(TValue item)
        {
            return m_InnerLIst.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            m_InnerLIst.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            m_InnerLIst.RemoveAt(index);
        }

        public void Clear()
        {
            m_InnerLIst.Clear();
        }

        public bool Contains(TValue item)
        {
            return m_InnerLIst.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            m_InnerLIst.CopyTo(array,arrayIndex);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return m_InnerLIst.GetEnumerator();
        }
    }
}
