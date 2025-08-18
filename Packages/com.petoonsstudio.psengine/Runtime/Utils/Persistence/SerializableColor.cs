using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        /// <summary>
        /// Serializable Color constructor
        /// </summary>
        /// <param name="rR"></param>
        /// <param name="rG"></param>
        /// <param name="rB"></param>
        /// <param name="rA"></param>
        public SerializableColor(float rR, float rG, float rB, float rA)
        {
            r = rR;
            g = rG;
            b = rB;
            a = rA;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", r, g, b);
        }

        /// <summary>
        /// Automatic conversion from SerializableVector3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Color(SerializableColor rValue)
        {
            return new Color(rValue.r, rValue.g, rValue.b, rValue.a);
        }

        /// <summary>
        /// Automatic conversion from Vector3 to SerializableVector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableColor(Color rValue)
        {
            return new SerializableColor(rValue.r, rValue.g, rValue.b, rValue.a);
        }
    }
}