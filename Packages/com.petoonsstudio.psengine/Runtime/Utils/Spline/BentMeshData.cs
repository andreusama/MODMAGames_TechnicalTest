using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{

    /// <summary>
    /// This class returns a transformed version of a given source mesh, plus others
    /// informations to help bending the mesh along a curve.
    /// It is imutable to ensure better performances.
    /// 
    /// To obtain an instance, call the static method <see cref="Build(Mesh)"/>.
    /// The building is made in a fluent way.
    /// </summary>
    public struct BentMeshData
    {
        private Vector3 translation;
        private Quaternion rotation;
        private Vector3 scale;

        internal Mesh Mesh { get; }

        private List<BentMeshVertex> vertices;
        internal List<BentMeshVertex> Vertices
        {
            get
            {
                if (vertices == null) BuildData();
                return vertices;
            }
        }

        private int[] triangles;
        internal int[] Triangles
        {
            get
            {
                if (vertices == null) BuildData();
                return triangles;
            }
        }

        private float minX;
        internal float MinX
        {
            get
            {
                if (vertices == null) BuildData();
                return minX;
            }
        }

        private float length;
        internal float Length
        {
            get
            {
                if (vertices == null) BuildData();
                return length;
            }
        }

        /// <summary>
        /// constructor is private to enable fluent builder pattern.
        /// Use <see cref="Build(Mesh)"/> to obtain an instance.
        /// </summary>
        /// <param name="mesh"></param>
        private BentMeshData(Mesh mesh)
        {
            Mesh = mesh;
            translation = default(Vector3);
            rotation = default(Quaternion);
            scale = default(Vector3);
            vertices = null;
            triangles = null;
            minX = 0;
            length = 0;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="other"></param>
        private BentMeshData(BentMeshData other)
        {
            Mesh = other.Mesh;
            translation = other.translation;
            rotation = other.rotation;
            scale = other.scale;
            vertices = null;
            triangles = null;
            minX = 0;
            length = 0;
        }

        public static BentMeshData Build(Mesh mesh)
        {
            return new BentMeshData(mesh);
        }

        public BentMeshData Translate(Vector3 translation)
        {
            var res = new BentMeshData(this)
            {
                translation = translation
            };
            return res;
        }

        public BentMeshData Translate(float x, float y, float z)
        {
            return Translate(new Vector3(x, y, z));
        }

        public BentMeshData Rotate(Quaternion rotation)
        {
            var res = new BentMeshData(this)
            {
                rotation = rotation
            };
            return res;
        }

        public BentMeshData Scale(Vector3 scale)
        {
            var res = new BentMeshData(this)
            {
                scale = scale
            };
            return res;
        }

        public BentMeshData Scale(float x, float y, float z)
        {
            return Scale(new Vector3(x, y, z));
        }

        private void BuildData()
        {
            // if the mesh is reversed by scale, we must change the culling of the faces by inversing all triangles.
            // the mesh is reverse only if the number of resersing axes is impair.
            bool reversed = scale.x < 0;
            if (scale.y < 0) reversed = !reversed;
            if (scale.z < 0) reversed = !reversed;
            triangles = reversed ? SplineMeshUtils.GetReversedTriangles(Mesh) : Mesh.triangles;

            // we transform the source mesh vertices according to rotation/translation/scale
            int i = 0;
            vertices = new List<BentMeshVertex>(Mesh.vertexCount);
            foreach (Vector3 vert in Mesh.vertices)
            {
                var transformed = new BentMeshVertex(vert, Mesh.normals[i++]);
                //  application of rotation
                if (rotation != Quaternion.identity)
                {
                    transformed.Position = rotation * transformed.Position;
                    transformed.Normal = rotation * transformed.Normal;
                }
                if (scale != Vector3.one)
                {
                    transformed.Position = Vector3.Scale(transformed.Position, scale);
                    transformed.Normal = Vector3.Scale(transformed.Normal, scale);
                }
                if (translation != Vector3.zero)
                {
                    transformed.Position += translation;
                }
                vertices.Add(transformed);
            }

            // find the bounds along x
            minX = float.MaxValue;
            float maxX = float.MinValue;
            foreach (var vert in vertices)
            {
                Vector3 p = vert.Position;
                maxX = Math.Max(maxX, p.x);
                minX = Math.Min(minX, p.x);
            }
            length = Math.Abs(maxX - minX);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (BentMeshData)obj;
            return Mesh == other.Mesh &&
                translation == other.translation &&
                rotation == other.rotation &&
                scale == other.scale;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(BentMeshData sm1, BentMeshData sm2)
        {
            return sm1.Equals(sm2);
        }
        public static bool operator !=(BentMeshData sm1, BentMeshData sm2)
        {
            return sm1.Equals(sm2);
        }
    }
}
