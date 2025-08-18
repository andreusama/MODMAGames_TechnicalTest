using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [System.Serializable]
    public class Ellipse
    {
        public Vector3 Center;
        public Vector3 RightAnchor;
        public Vector3 UpAnchor;
        public bool LockedCenter = true;

        public float Width => Vector3.Distance(Center, RightAnchor);
        public float Height => Vector3.Distance(Center, UpAnchor);
        public Vector3 UpDir => (UpAnchor - Center).normalized;
        public Vector3 RightDir => (RightAnchor - Center).normalized;

#if UNITY_EDITOR
        public const int DEFAULT_DRAW_SUBDIV = 100;
#endif

        public Ellipse(Vector3 center, float width = 1f, float height = 1f, bool lockedCenter = true)
        {
            Center = center;
            RightAnchor = center + Vector3.right * width;
            UpAnchor = center + Vector3.up * height;
            LockedCenter = lockedCenter;
        }

        public Ellipse(Ellipse other)
        {
            Center = other.Center;
            RightAnchor = other.RightAnchor;
            UpAnchor = other.UpAnchor;
            LockedCenter = other.LockedCenter;
        }

        public void SetCenter(Vector3 newCenter)
        {
            if (LockedCenter)
            {
                Vector3 diff = newCenter - Center;
                RightAnchor += diff;
                UpAnchor += diff;
            }

            Center = newCenter;
        }

        public void SetWidth(float width)
        {
            RightAnchor = Center + RightDir * width;
        }

        public void SetHeight(float height)
        {
            UpAnchor = Center + UpDir * height;
        }

        public List<Vector3> GetAsPoints(int subdivisions)
        {
            List<Vector3> points = new List<Vector3>();

            float width = Width;
            float height = Height;

            Vector3 upDir = UpDir;
            Vector3 rightDir = RightDir;

            float x, y;

            float div = (Mathf.PI * 2f) / subdivisions;


            for (float i = 0; i < Mathf.PI * 2f; i += div)
            {
                x = Mathf.Sin(i);
                y = Mathf.Cos(i);

                points.Add(Center + rightDir * x * width + upDir * y * height);
            }

            points.Add(UpAnchor);

            return points;
        }

        public Vector3 GetPoint(float t)
        {
            float angle = Mathf.PI * 2f * t;
            return Center + RightDir * Width * Mathf.Sin(angle) + UpDir * Height * Mathf.Cos(angle);
        }

#if UNITY_EDITOR
        public void DrawEllipseHandles()
        {
            var ellipsePoints = GetAsPoints(DEFAULT_DRAW_SUBDIV);

            Vector3 prevPoint = UpAnchor;
            foreach (var point in ellipsePoints)
            {
                Handles.DrawLine(prevPoint, point);
                prevPoint = point;
            }

            Handles.DrawLine(prevPoint, UpAnchor);

            EditorGUI.BeginChangeCheck();

            Vector3 ellipseCenter = Handles.PositionHandle(Center, Quaternion.identity);
            Vector3 ellipseRight = Handles.PositionHandle(RightAnchor, Quaternion.identity);
            Vector3 ellipseUp = Handles.PositionHandle(UpAnchor, Quaternion.identity);

            Handles.Label(Center, "Center");
            Handles.Label(RightAnchor, "Width");
            Handles.Label(UpAnchor, "Height");

            if (EditorGUI.EndChangeCheck())
            {
                if (LockedCenter)
                {
                    Vector3 diff = ellipseCenter - Center;

                    RightAnchor = ellipseCenter + RightDir * Vector3.Distance(ellipseCenter, ellipseRight + diff);
                    UpAnchor = ellipseCenter + UpDir * Vector3.Distance(ellipseCenter, ellipseUp + diff);
                }
                else
                {
                    RightAnchor = ellipseRight;
                    UpAnchor = ellipseUp;
                }

                Center = ellipseCenter;
            }

            Color prevColor = Handles.color;
            Handles.color = Color.red;
            Handles.DrawLine(Center, RightAnchor);
            Handles.color = Color.green;
            Handles.DrawLine(Center, UpAnchor);
            Handles.color = prevColor;
        }
#endif
    } 
}
