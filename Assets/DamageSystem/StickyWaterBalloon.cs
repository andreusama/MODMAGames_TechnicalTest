using UnityEngine;

public class StickyWaterBalloon : WaterBalloon
{
    protected override void OnCollisionEnter(Collision collision)
    {
        if (m_HasTouchedGround || m_HasExploded)
            return;

        // Comprueba si el objeto con el que colisiona está en las capas de suelo
        if (((1 << collision.gameObject.layer) & GroundLayers.value) != 0)
        {
            m_HasTouchedGround = true;

            // Detiene el movimiento y "pega" el globo al punto de impacto
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Ajusta la posición al punto de contacto más cercano
            if (collision.contacts.Length > 0)
            {
                transform.position = collision.contacts[0].point;
            }

            m_ExplosionCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }
}