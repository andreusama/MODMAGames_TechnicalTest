using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils.Graph
{

    [Serializable]
    public class Graph<TVertexValue> where TVertexValue : class
    {
        [SerializeField]
        protected List<Vertex<TVertexValue>> m_Vertices = new List<Vertex<TVertexValue>>();
        [SerializeField]
        protected SerializedDictionary<string, AdjacencyList<TVertexValue>> m_AdjacencyList =
            new SerializedDictionary<string, AdjacencyList<TVertexValue>>();
        public List<Vertex<TVertexValue>> Vertices { get { return m_Vertices; } }
        public SerializedDictionary<string, AdjacencyList<TVertexValue>> AdjacencyList { get { return m_AdjacencyList; } }

        public Graph() { }
        public Graph(List<Vertex<TVertexValue>> vertices)
        {
            this.m_Vertices = vertices;
        }

        public virtual Vertex<TVertexValue> AddVertex(TVertexValue value)
        {
            var vertex = new Vertex<TVertexValue>(value);

            return AddVertex(vertex);
        }

        public virtual Vertex<TVertexValue> AddVertex(Vertex<TVertexValue> vertex)
        {
            m_Vertices.Add(vertex);
            m_AdjacencyList.Add(vertex.Id, new AdjacencyList<TVertexValue>());

            return vertex;
        }

        public virtual void AddEdge(WeightedEdge<TVertexValue> edge)
        {
            if (!ContainsVertex(edge.Start.Id) || !ContainsVertex(edge.End.Id))
            {
                Debug.LogWarning("Adding an edge to an unknow vertex");
            }

            m_AdjacencyList[edge.Start.Id].AddOutEdge(edge);
            m_AdjacencyList[edge.End.Id].AddInEdge(edge);
        }
        public WeightedEdge<TVertexValue> AddEdge(string startCellID, string endCellId, int weight = 0)
        {
            Vertex<TVertexValue> startVertex = GetVertex(startCellID);
            Vertex<TVertexValue> endVertex = GetVertex(endCellId);
            WeightedEdge<TVertexValue> edge = new WeightedEdge<TVertexValue>(startVertex, endVertex, weight);
            AddEdge(edge);

            return edge;
        }

        public Vertex<TVertexValue> GetVertex(Vertex<TVertexValue> vertex)
        {
            return GetVertex(vertex.Id);
        }
        public Vertex<TVertexValue> GetVertex(string vertexId)
        {
            return m_Vertices.Find(x => x.Id.Equals(vertexId));
        }

        public virtual void RemoveVertex(Vertex<TVertexValue> vertex)
        {
            RemoveVertex(vertex.Id);
        }
        public virtual void RemoveVertex(string vertexId)
        {
            if (!TryGetVertex(vertexId, out var vertex)) return;

            m_Vertices.Remove(vertex);
            RemoveEdges(vertex);
            m_AdjacencyList.Remove(vertex.Id);
        }
        public virtual void RemoveEdge(WeightedEdge<TVertexValue> edge)
        {
            m_AdjacencyList[edge.Start.Id].RemoveOutEdge(edge);
            m_AdjacencyList[edge.End.Id].RemoveInEdge(edge);
        }
        public virtual void RemoveEdges(Vertex<TVertexValue> vertex)
        {
            RemoveEdges(vertex.Id);
        }
        public virtual void RemoveEdges(string vertexId)
        {
            foreach (var edge in m_AdjacencyList[vertexId].EdgesList)
            {
                RemoveEdge(edge);
            }
        }

        public bool ContainsVertex(string vertexId)
        {
            return m_Vertices.Find(x => x.Id.Equals(vertexId)) != null;
        }

        public bool TryGetVertex(Vertex<TVertexValue> vertex, out Vertex<TVertexValue> outVertex)
        {
            return TryGetVertex(vertex.Id, out outVertex);
        }
        public bool TryGetVertex(string vertexId, out Vertex<TVertexValue> vertex)
        {
            vertex = m_Vertices.Find(x => x.Id.Equals(vertexId));

            return vertex != null;
        }
        #region SEARCH
        public Dictionary<string, bool> BreathFirstSearch(Vertex<TVertexValue> start)
        {
            Dictionary<string, bool> visited = new Dictionary<string, bool>();

            foreach (var vertexId in m_AdjacencyList.Keys)
            {
                visited.Add(vertexId, false);
            }

            BFSVisitNeighbors(start, ref visited);

            return visited;
        }
        public void BFSVisitNeighbors(Vertex<TVertexValue> vertex, ref Dictionary<string, bool> visited)
        {
            if (visited[vertex.Id]) return;
            visited[vertex.Id] = true;

            var outEdges = m_AdjacencyList[vertex.Id].OutEdges;

            foreach (var edge in outEdges.Values)
            {
                Vertex<TVertexValue> neighbor = edge.End;
                if (visited[neighbor.Id]) continue;
                BFSVisitNeighbors(neighbor, ref visited);
            }
        }

        public List<TVertexValue> Dijkstra(Vertex<TVertexValue> startVertex, Vertex<TVertexValue> endVertex)
        {
            List<TVertexValue> result = new List<TVertexValue>();
            Dictionary<string, string> parentMap = new Dictionary<string, string>();
            SortedDictionary<string, int> minHeap = new SortedDictionary<string, int>();

            foreach (var node in m_Vertices)
            {
                minHeap.Add(node.Id, int.MaxValue);
            }
            minHeap[startVertex.Id] = 0;

            Vertex<TVertexValue> current = startVertex;

            CustomPriorityQueue<Vertex<TVertexValue>> priorityQueue = new CustomPriorityQueue<Vertex<TVertexValue>>();
            priorityQueue.Enqueue(startVertex, 0);



            while (priorityQueue.Count > 0)
            {
                current = priorityQueue.Dequeue();

                if (parentMap.ContainsKey(current.Id))
                    continue;

                if (current.Id.Equals(endVertex.Id))
                {
                    break;
                }

                foreach (WeightedEdge<TVertexValue> edge in m_AdjacencyList[current.Id].OutEdges.Values)
                {
                    Vertex<TVertexValue> neighbor = edge.End;

                    int newCost = minHeap[current.Id] + edge.Weight;

                    if (minHeap[neighbor.Id] > newCost)
                    {
                        priorityQueue.Enqueue(neighbor, newCost);
                        minHeap[neighbor.Id] = newCost;
                        parentMap[current.Id] = neighbor.Id;
                    }

                    if (neighbor.Id.Equals(endVertex.Id))
                    {
                        current = neighbor;
                        break;
                    }
                }
            }

            if (!parentMap.ContainsValue(endVertex.Id))
                return null;

            Queue<string> resultQueue = new Queue<string>();
            resultQueue.Enqueue(startVertex.Id);

            string currentId = startVertex.Id;
            result.Add(GetVertex(currentId).Value);

            while (!currentId.Equals(endVertex.Id))
            {
                currentId = parentMap[currentId];
                result.Add(GetVertex(currentId).Value);
            }

            return result;
        }
        #endregion
        public virtual void Clear()
        {
            m_Vertices.Clear();
            m_AdjacencyList.Clear();
        }
    }

}
