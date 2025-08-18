using UnityEngine;
using UnityEngine.Splines;

namespace PetoonsStudio.PSEngine.Utils
{
    public struct CurveData
    {
        public readonly Vector3 Position;
        public readonly Vector3 Tangent;
        public readonly Vector3 Up;
        public readonly Vector2 Scale;
        public readonly float Roll;
        public readonly Spline Spline;

        private Quaternion rotation;

        private const float MIN_TANGENT_MARGIN = 0.001f;
        private const float MAX_TANGENT_MARGIN = 0.995f;

        /// <summary>
        /// Rotation is a look-at quaternion calculated from the tangent, roll and up vector. Mixing non zero roll and custom up vector is not advised.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                if (rotation == Quaternion.identity)
                {
                    var upVector = Vector3.Cross(Tangent, Vector3.Cross(Quaternion.AngleAxis(Roll, Vector3.forward) * Up, Tangent).normalized);
                    rotation = Quaternion.LookRotation(Tangent, upVector);
                }
                return rotation;
            }
        }

        public CurveData(Vector3 position, Vector3 tangent, Vector3 up, Vector2 scale, float roll, Spline spline)
        {
            this.Position = position;
            this.Tangent = tangent;
            this.Up = up;
            this.Roll = roll;
            this.Scale = scale;
            this.Spline = spline;
            rotation = Quaternion.identity;
        }

        public CurveData(SplineContainer spline, float distance)
        {
            //Evaluate Tangent of 0, returns Tangent Vector.zero instead of the current value so we use a margin value in order to avoid return value of zero.
            var splineRelativeDistance = MMMaths.Remap(distance, 0, spline.Spline.GetLength(), MIN_TANGENT_MARGIN, MAX_TANGENT_MARGIN);

            this.Position = SplineUtility.EvaluatePosition(spline.Spline, splineRelativeDistance);
            this.Tangent = SplineUtility.EvaluateTangent(spline.Spline, splineRelativeDistance);
            this.Up = SplineUtility.EvaluateUpVector(spline.Spline, splineRelativeDistance); ;
            this.Roll = 0f;
            this.Scale = Vector2.one;
            this.Spline = spline.Spline;
            rotation = Quaternion.identity;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            CurveData other = (CurveData)obj;
            return Position == other.Position &&
                Tangent == other.Tangent &&
                Up == other.Up &&
                Scale == other.Scale &&
                Roll == other.Roll;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CurveData cs1, CurveData cs2)
        {
            return cs1.Equals(cs2);
        }

        public static bool operator !=(CurveData cs1, CurveData cs2)
        {
            return !cs1.Equals(cs2);
        }

        public BentMeshVertex GetBent(BentMeshVertex vert)
        {
            var res = new BentMeshVertex(vert.Position, vert.Normal, vert.UV);

            // application of scale
            res.Position = Vector3.Scale(res.Position, new Vector3(0, Scale.y, Scale.x));

            // application of roll
            res.Position = Quaternion.AngleAxis(Roll, Vector3.right) * res.Position;
            res.Normal = Quaternion.AngleAxis(Roll, Vector3.right) * res.Normal;

            // reset X value
            res.Position.x = 0;

            // application of the rotation + location
            Quaternion q = Rotation * Quaternion.Euler(0, -90, 0);
            res.Position = q * res.Position + Position;
            res.Normal = q * res.Normal;
            return res;
        }
    }
}
