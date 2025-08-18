using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class BentMeshVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public BentMeshVertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        public BentMeshVertex(Vector3 position, Vector3 normal)
            : this(position, normal, Vector2.zero)
        {
        }
    }
}
