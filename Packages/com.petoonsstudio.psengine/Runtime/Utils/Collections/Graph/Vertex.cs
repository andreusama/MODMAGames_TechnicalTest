using UnityEngine;
using System;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Utils.Graph
{
    [Serializable]
    public class Vertex<TVertexValue>
    {
        [ReadOnly, SerializeField]
        private string m_Id = Guid.NewGuid().ToString();
        [SerializeField]
        private TVertexValue m_Value;

        public string Id => m_Id;
        public TVertexValue Value { get { return m_Value; } set { this.m_Value = value; } }

        public Vertex(TVertexValue value)
        {
            this.m_Value = value;
        }

        public Vertex(string id, TVertexValue value)
        {
            m_Id = id;
            Value = value;
        }   
    }
}
