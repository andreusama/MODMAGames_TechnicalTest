#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Splines;
using UnityEditor;
#endif
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// A component that creates a deformed mesh from a given one along the given spline segment.
    /// The source mesh will always be bended along the X axis.
    /// It can work on a cubic bezier curve or on any interval of a given spline.
    /// On the given interval, the mesh can be place with original scale, stretched, or repeated.
    /// The resulting mesh is stored in a MeshFilter component and automaticaly updated on the next update if the spline segment change.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class SplineMeshBender : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// The mode used by <see cref="PathMeshBender"/> to bend meshes on the interval.
        /// </summary>
        public enum FillingMode
        {
            /// <summary>
            /// In this mode, source mesh will be placed on the interval by preserving mesh scale.
            /// Vertices that are beyond interval end will be placed on the interval end.
            /// </summary>
            Once,
            /// <summary>
            /// In this mode, the mesh will be repeated to fill the interval, preserving
            /// mesh scale.
            /// This filling process will stop when the remaining space is not enough to
            /// place a whole mesh, leading to an empty interval.
            /// </summary>
            Repeat,
            /// <summary>
            /// In this mode, the mesh is deformed along the X axis to fill exactly the interval.
            /// </summary>
            StretchToInterval
        }

        private SplineContainer m_SplineContainer;
        private BentMeshData m_SourceMesh;
        private Mesh m_FinalMesh;
        private Dictionary<float, CurveData> m_SampleCache = new();
        [SerializeField]
        private FillingMode m_Mode = FillingMode.StretchToInterval;
        [SerializeField]
        private float m_IntervalStart, m_IntervalEnd;
        private uint? m_RepetitionCount;
        private bool m_IsDirty = false;

        public float IntervalStart { get => m_IntervalStart; }
        public float IntervalEnd { get => m_IntervalEnd; }

        /// <summary>
        /// The source mesh to bend.
        /// </summary>
        public BentMeshData Source
        {
            get { return m_SourceMesh; }
            set
            {
                if (value == m_SourceMesh) return;
                SetDirty();
                m_SourceMesh = value;
            }
        }

        /// <summary>
        /// The scaling mode along the spline
        /// </summary>
        public FillingMode Mode
        {
            get { return m_Mode; }
            set
            {
                if (value == m_Mode) return;
                SetDirty();
                m_Mode = value;
            }
        }

        /// <summary>
        /// Sets a spline's interval along which the mesh will be bent.
        /// If interval end is absent or set to 0, the interval goes from start to spline length.
        /// The mesh will be update if any of the curve changes on the spline, including curves
        /// outside the given interval.
        /// </summary>
        /// <param name="splineContainer">The <see cref="PathCreator"/> to bend the source mesh along.</param>
        /// <param name="intervalStart">Distance from the spline start to place the mesh minimum X.<param>
        /// <param name="intervalEnd">Distance from the spline start to stop deforming the source mesh.</param>
        public void SetInterval(SplineContainer splineContainer, float intervalStart, float intervalEnd = 0, uint? repetitionCount = null)
        {
            if (splineContainer == null) throw new ArgumentNullException(nameof(splineContainer));
            if (intervalStart < 0 || intervalStart > splineContainer.Spline.GetLength())
            {
                throw new ArgumentOutOfRangeException("interval start must be 0 or greater and lesser than spline length (was " + intervalStart + ")");
            }

            this.m_SplineContainer = splineContainer;

            this.m_IntervalStart = intervalStart;
            this.m_IntervalEnd = intervalEnd;
            m_RepetitionCount = repetitionCount;
            SetDirty();
        }

        private void OnEnable()
        {
            if (GetComponent<MeshFilter>().sharedMesh != null)
            {
                m_FinalMesh = GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                GetComponent<MeshFilter>().sharedMesh = m_FinalMesh = new Mesh();
                m_FinalMesh.name = "Generated_by_" + GetType().Name;
            }
        }

        private void LateUpdate()
        {
            ComputeIfNeeded();
        }

        public void ComputeIfNeeded()
        {
            if (m_IsDirty)
            {
                Compute();
            }
        }

        private void SetDirty()
        {
            m_IsDirty = true;
        }

        /// <summary>
        /// Bend the mesh. This method may take time and should not be called more than necessary.
        /// Consider using <see cref="ComputeIfNeeded"/> for faster result.
        /// </summary>
        private void Compute()
        {
            m_IsDirty = false;
            switch (Mode)
            {
                case FillingMode.Once:
                    FillOnce();
                    break;
                case FillingMode.Repeat:
                    FillRepeat();
                    break;
                case FillingMode.StretchToInterval:
                    FillStretch();
                    break;
            }
        }

        private void FillOnce()
        {
            m_SampleCache.Clear();
            var bentVertices = new List<BentMeshVertex>(m_SourceMesh.Vertices.Count);

            // for each mesh vertex, we found its projection on the curve
            foreach (var vert in m_SourceMesh.Vertices)
            {
                float distance = vert.Position.x - m_SourceMesh.MinX;
                distance = (float)Math.Round(distance, 3);
                CurveData data;

                if (!m_SampleCache.TryGetValue(distance, out data))
                {
                    float distOnSpline = m_IntervalStart * m_SplineContainer.Spline.GetLength() + distance;
                    if (distOnSpline > m_SplineContainer.Spline.GetLength())
                    {
                        if (m_SplineContainer.Spline.Closed)
                        {
                            while (distOnSpline > m_SplineContainer.Spline.GetLength())
                            {
                                distOnSpline -= m_SplineContainer.Spline.GetLength();
                            }
                        }
                        else
                        {
                            distOnSpline = m_SplineContainer.Spline.GetLength();
                        }
                    }
                    data = new CurveData(m_SplineContainer, distOnSpline);
                    m_SampleCache[distance] = data;
                }

                bentVertices.Add(data.GetBent(vert));
            }

            SplineMeshUtils.UpdateMesh(m_FinalMesh,
                m_SourceMesh.Mesh,
                m_SourceMesh.Triangles,
                bentVertices.Select(b => b.Position),
                bentVertices.Select(b => b.Normal));
        }

        private class SubMesh
        {
            // building triangles and UVs for the repeated mesh
            public List<int> triangles = new List<int>();
            public List<Vector2> uv = new List<Vector2>();
            public List<Vector2> uv2 = new List<Vector2>();
            public List<Vector2> uv3 = new List<Vector2>();
            public List<Vector2> uv4 = new List<Vector2>();
            public List<Vector2> uv5 = new List<Vector2>();
            public List<Vector2> uv6 = new List<Vector2>();
            public List<Vector2> uv7 = new List<Vector2>();
            public List<Vector2> uv8 = new List<Vector2>();

            public List<BentMeshVertex> bentVertices = new List<BentMeshVertex>();

            public SubMesh(int bentVerticesCount)
            {
                bentVertices = new List<BentMeshVertex>(bentVerticesCount);
            }
        }

        private void FillRepeat()
        {
            float intervalLength = (m_IntervalEnd == 0 ? m_SplineContainer.Spline.GetLength() : m_IntervalEnd) - m_IntervalStart;
            uint? repetitionCount;
            if (m_RepetitionCount == null)
            {
                var numberOfMeshes = intervalLength / m_SourceMesh.Length;
                repetitionCount = (uint)Mathf.FloorToInt(numberOfMeshes);
            }
            else
                repetitionCount = m_RepetitionCount;


            // Generate mesh for every submesh of Source Mesh, and save it on a list
            int submeshesCount = m_SourceMesh.Mesh.subMeshCount;
            List<Mesh> generatedSubMeshes = new(submeshesCount);
            for (int submesh = 0; submesh < submeshesCount; submesh++)
            {
                var mesh = new SubMesh(m_SourceMesh.Vertices.Count);
                int[] subMeshTriangles = m_SourceMesh.Mesh.GetTriangles(submesh);

                for (int i = 0; i < repetitionCount; i++)
                {
                    for (int y = 0; y < subMeshTriangles.Length; y++)
                    {
                        mesh.triangles.Add(subMeshTriangles[y] + m_SourceMesh.Vertices.Count * i);
                    }

                    mesh.uv.AddRange(m_SourceMesh.Mesh.uv);
                    mesh.uv2.AddRange(m_SourceMesh.Mesh.uv2);
                    mesh.uv3.AddRange(m_SourceMesh.Mesh.uv3);
                    mesh.uv4.AddRange(m_SourceMesh.Mesh.uv4);
#if UNITY_2018_2_OR_NEWER
                    mesh.uv5.AddRange(m_SourceMesh.Mesh.uv5);
                    mesh.uv6.AddRange(m_SourceMesh.Mesh.uv6);
                    mesh.uv7.AddRange(m_SourceMesh.Mesh.uv7);
                    mesh.uv8.AddRange(m_SourceMesh.Mesh.uv8);
#endif
                }

                // computing vertices and normals
                var bentVertices = new List<BentMeshVertex>(m_SourceMesh.Vertices.Count);
                float offset = 0;
                for (int i = 0; i < repetitionCount; i++)
                {
                    m_SampleCache.Clear();
                    // for each mesh vertex, we found its projection on the curve
                    foreach (var vert in m_SourceMesh.Vertices)
                    {
                        float distance = vert.Position.x - m_SourceMesh.MinX + offset;
                        CurveData sample;
                        if (!m_SampleCache.TryGetValue(distance, out sample))
                        {
                            float distOnSpline = m_IntervalStart + distance;
                            while (distOnSpline > m_SplineContainer.Spline.GetLength())
                            {
                                distOnSpline -= m_SplineContainer.Spline.GetLength();
                            }

                            sample = new CurveData(m_SplineContainer, distOnSpline);

                            m_SampleCache[distance] = sample;
                        }
                        bentVertices.Add(sample.GetBent(vert));
                    }
                    offset += m_SourceMesh.Length;
                }

                Mesh auxMesh = new Mesh();
                SplineMeshUtils.UpdateMesh(auxMesh,
                    m_SourceMesh.Mesh,
                    mesh.triangles,
                    bentVertices.Select(b => b.Position),
                    bentVertices.Select(b => b.Normal),
                    mesh.uv,
                    mesh.uv2,
                    mesh.uv3,
                    mesh.uv4,
                    mesh.uv5,
                    mesh.uv6,
                    mesh.uv7,
                    mesh.uv8);

                generatedSubMeshes.Add(auxMesh);

            }

            // Combine meshes of the list
            CombineInstance[] combineInstances = new CombineInstance[submeshesCount];
            for (int i = 0; i < generatedSubMeshes.Count; i++)
            {
                combineInstances[i] = new CombineInstance();
                combineInstances[i].mesh = generatedSubMeshes[i];
                combineInstances[i].subMeshIndex = 0;
            }
            Mesh combMesh = new Mesh();
            m_FinalMesh.CombineMeshes(combineInstances, false, false);

        }

        private void FillStretch()
        {
            var bentVertices = new List<BentMeshVertex>(m_SourceMesh.Vertices.Count);
            m_SampleCache.Clear();
            // for each mesh vertex, we found its projection on the curve
            foreach (var vert in m_SourceMesh.Vertices)
            {
                float distanceRate = m_SourceMesh.Length == 0 ? 0 : Math.Abs(vert.Position.x - m_SourceMesh.MinX) / m_SourceMesh.Length;
                CurveData sample;
                if (!m_SampleCache.TryGetValue(distanceRate, out sample))
                {
                    float intervalLength = m_IntervalEnd == 0 ? m_SplineContainer.Spline.GetLength() - m_IntervalStart : m_IntervalEnd - m_IntervalStart;
                    float distOnSpline = m_IntervalStart + intervalLength * distanceRate;
                    if (distOnSpline > m_SplineContainer.Spline.GetLength())
                    {
                        distOnSpline = m_SplineContainer.Spline.GetLength();
                    }

                    sample = new CurveData(m_SplineContainer, distOnSpline);

                    m_SampleCache[distanceRate] = sample;
                }

                bentVertices.Add(sample.GetBent(vert));
            }

            SplineMeshUtils.UpdateMesh(m_FinalMesh,
                m_SourceMesh.Mesh,
                m_SourceMesh.Triangles,
                bentVertices.Select(b => b.Position),
                bentVertices.Select(b => b.Normal));

            if (TryGetComponent(out MeshCollider collider))
            {
                collider.sharedMesh = m_FinalMesh;
            }
        }
#endif
    }
}
