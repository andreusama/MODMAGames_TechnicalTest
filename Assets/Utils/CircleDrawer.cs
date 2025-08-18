using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleDrawer : MonoBehaviour
{
    public int Segments = 32;
    public int ParabolicSegments = 32;
    public float Radius = 1f;
    private LineRenderer m_LineRenderer;
    public LineRenderer m_ParabolicLineRenderer;
    public LayerMask CollisionMask = ~0; // Por defecto, colisiona con todo

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.loop = true;
        m_LineRenderer.useWorldSpace = true;
        m_LineRenderer.positionCount = Segments;
    }

    /// <summary>
    /// Dibuja un círculo en el punto de colisión más cercano bajo el centro, o en el centro si no hay colisión.
    /// </summary>
    public void DrawCircle(Vector3 center, float radius)
    {
        // Lanza un rayo hacia abajo desde el centro para encontrar la altura de la superficie
        Vector3 circleCenter = center;
        if (Physics.Raycast(center + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, CollisionMask))
        {
            circleCenter.y = hit.point.y + 0.1f; // Ligeramente por encima de la superficie
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
    /// Dibuja una parábola hasta el primer punto de colisión.
    /// </summary>
    public void DrawParabola(Vector3 start, Vector3 initialVelocity, float gravity, float maxTime, int steps = 30)
    {
        if (m_ParabolicLineRenderer == null)
            return;

        Vector3 prevPos = start;
        m_ParabolicLineRenderer.positionCount = 0;
        m_ParabolicLineRenderer.loop = false;

        int pointIndex = 0;
        for (int i = 0; i < ParabolicSegments; i++)
        {
            float t = (i / (float)(ParabolicSegments - 1)) * maxTime;
            Vector3 pos = start + initialVelocity * t + 0.5f * Physics.gravity * t * t;

            // Añade el punto a la línea
            if (m_ParabolicLineRenderer.positionCount <= pointIndex)
                m_ParabolicLineRenderer.positionCount = pointIndex + 1;
            m_ParabolicLineRenderer.SetPosition(pointIndex, pos);

            // Comprueba colisión entre prevPos y pos
            if (Physics.Linecast(prevPos, pos, out RaycastHit hit, CollisionMask))
            {
                // Ajusta el último punto a la superficie de colisión
                m_ParabolicLineRenderer.SetPosition(pointIndex, hit.point);
                m_ParabolicLineRenderer.positionCount = pointIndex + 1;
                break;
            }

            prevPos = pos;
            pointIndex++;
        }
    }

    public void Hide()
    {
        m_LineRenderer.positionCount = 0;
        if (m_ParabolicLineRenderer != null)
        {
            m_ParabolicLineRenderer.positionCount = 0;
        }
    }
}