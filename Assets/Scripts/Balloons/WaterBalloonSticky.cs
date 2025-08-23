using UnityEngine;

public class WaterBalloonSticky : WaterBalloon
{
    protected override void OnTouchedGroundHook(Collision collision)
    {
        if (m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.linearVelocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
        }

        // In addition to base behavior (reposition with normal)
        base.OnTouchedGroundHook(collision);
    }
}