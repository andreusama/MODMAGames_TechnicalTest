using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleDrawer : MonoBehaviour
{
    public int Segments = 32;
    public int ParabolicSegments = 32;
    public float Radius = 1f;
    private LineRenderer m_LineRenderer;
    public LineRenderer ParabolicLineRenderer;
    public LayerMask CollisionMask = ~0; // By default, collides with everything

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.loop = true;
        m_LineRenderer.useWorldSpace = true;
        m_LineRenderer.positionCount = Segments;
    }

    /// <summary>
    /// Draws a circle at the closest collision point below the center, or at the center if no collision.
    /// </summary>
    public void DrawCircle(Vector3 center, float radius)
    {
        // Cast a ray downward from the center to find surface height
        Vector3 circleCenter = center;
        if (Physics.Raycast(center + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, CollisionMask))
        {
            circleCenter.y = hit.point.y + 0.1f; // Slightly above surface
        }

        m_LineRenderer.positionCount = Segments;
        m_LineRenderer.loop = true;
        for (int i = 0; i < Segments; i++)
        {
            float angle = 2 * Mathf.PI * i / Segments;
            Vector3 pos = circleCenter + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            m_LineRenderer.SetPosition(i, pos);
        }
    }

    /// <summary>
    /// Draws a parabola until the first collision point.
    /// </summary>
    public void DrawParabola(Vector3 start, Vector3 initialVelocity, float gravity, float maxTime, int steps = 30)
    {
        if (ParabolicLineRenderer == null)
            return;

        Vector3 prevPos = start;
        ParabolicLineRenderer.positionCount = 0;
        ParabolicLineRenderer.loop = false;

        int pointIndex = 0;
        for (int i = 0; i < ParabolicSegments; i++)
        {
            float t = (i / (float)(ParabolicSegments - 1)) * maxTime;
            Vector3 pos = start + initialVelocity * t + 0.5f * Physics.gravity * t * t;

            // Add point to line
            if (ParabolicLineRenderer.positionCount <= pointIndex)
                ParabolicLineRenderer.positionCount = pointIndex + 1;
            ParabolicLineRenderer.SetPosition(pointIndex, pos);

            // Check collision between prevPos and pos
            if (Physics.Linecast(prevPos, pos, out RaycastHit hit, CollisionMask))
            {
                // Adjust last point to collision surface
                ParabolicLineRenderer.SetPosition(pointIndex, hit.point);
                ParabolicLineRenderer.positionCount = pointIndex + 1;
                break;
            }

            prevPos = pos;
            pointIndex++;
        }
    }

    /// <summary>
    /// Draws a parabola using a custom curve until the first collision point.
    /// </summary>
    public void DrawParabolaWithCurve(Vector3 start, Vector3 target, AnimationCurve curve, float totalFlightTime, int steps = 30)
    {
        if (ParabolicLineRenderer == null || curve == null)
            return;

        Vector3[] points = new Vector3[steps];
        for (int i = 0; i < steps; i++)
        {
            float tNorm = i / (float)(steps - 1);
            float curveT = curve.Evaluate(tNorm);
            points[i] = ParabolicCalculator.ParabolicLerp(start, target, curveT);
        }
        ParabolicLineRenderer.positionCount = steps;
        ParabolicLineRenderer.SetPositions(points);
    }

    public void Hide()
    {
        m_LineRenderer.positionCount = 0;
        if (ParabolicLineRenderer != null)
        {
            ParabolicLineRenderer.positionCount = 0;
        }
    }
}