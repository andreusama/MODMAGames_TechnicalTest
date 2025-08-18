using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils.Graph
{
    [Serializable]
    public class WeightedEdge<T>
    {
        [SerializeField]
        private string id;
        [SerializeField]
        private int m_Weight;

        [SerializeField]
        private Vertex<T> m_Start;
        [SerializeField]
        private Vertex<T> m_End;

        public string Id { get { return id; } }
        public int Weight { get { return m_Weight; } }
        public Vertex<T> Start { get { return m_Start; } }
        public Vertex<T> End { get { return m_End; } }

        public WeightedEdge(Vertex<T> start, Vertex<T> end, int weight)
        {
            id = Guid.NewGuid().ToString();
            this.m_Start = start;
            this.m_End = end;
            this.m_Weight = weight;
        }
    }
}
