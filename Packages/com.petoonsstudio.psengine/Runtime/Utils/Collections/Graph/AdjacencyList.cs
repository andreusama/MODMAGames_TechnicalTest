using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils.Graph
{
    [Serializable]
    public class AdjacencyList<TVertexValue>
    {
        [SerializeField]
        private SerializedDictionary<string,WeightedEdge<TVertexValue>> m_OutEdges = new();
        [SerializeField]
        private SerializedDictionary<string, WeightedEdge<TVertexValue>> m_InEdges = new();
        public SerializedDictionary<string, WeightedEdge<TVertexValue>> OutEdges
        {
            get
            {
                return m_OutEdges;
            }
        }
        public SerializedDictionary<string, WeightedEdge<TVertexValue>> InEdges
        {
            get
            {
                return m_InEdges;
            }
        }

        public List<WeightedEdge<TVertexValue>> OutEdgesList
        {
            get
            {
                List<WeightedEdge<TVertexValue>> edges = new();

                edges.AddRange(m_OutEdges.Values);

                return edges;
            }
        }
        public List<WeightedEdge<TVertexValue>> InEdgesList
        {
            get
            {
                List<WeightedEdge<TVertexValue>> edges = new();

                edges.AddRange(m_InEdges.Values);

                return edges;
            }
        }

        public List<WeightedEdge<TVertexValue>> EdgesList
        {
            get
            {
                List<WeightedEdge<TVertexValue>> edges = new();

                edges.AddRange(m_InEdges.Values);
                edges.AddRange(m_OutEdges.Values);

                return edges;
            }
        }

        public void AddOutEdge(WeightedEdge<TVertexValue> edge)
        {
            m_OutEdges.Add(edge.Id, edge);
        }
        public void AddInEdge(WeightedEdge<TVertexValue> edge)
        {
            m_InEdges.Add(edge.Id, edge);
        }

        public void RemoveOutEdge(WeightedEdge<TVertexValue> edge)
        {
            m_OutEdges.Remove(edge.Id);
        }
        public void RemoveInEdge(WeightedEdge<TVertexValue> edge)
        {
            m_InEdges.Remove(edge.Id);
        }

        public bool ContainsEdge(WeightedEdge<TVertexValue> edge)
        {
            return m_OutEdges.ContainsKey(edge.Id) || m_InEdges.ContainsKey(edge.Id);
        }
    }
}
