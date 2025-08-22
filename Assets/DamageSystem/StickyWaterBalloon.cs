using UnityEngine;

public class StickyWaterBalloon : WaterBalloon
{
    protected override void OnTouchedGroundHook(Collision collision)
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Además del comportamiento base (reposiciona con normal)
        base.OnTouchedGroundHook(collision);
    }
}