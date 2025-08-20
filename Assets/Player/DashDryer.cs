using UnityEngine;

[CreateAssetMenu(fileName = "DashDryer", menuName = "Skills/Dash Effects/Dash Dryer", order = 2)]
public class DashDryer : DashEffect
{
    public override void ApplyEffect(Collider other)
    {
        var wettable = other.GetComponent<IWettable>();
        if (wettable != null && wettable.Wetness > 0)
        {
            var explodable = other.GetComponent<IExplodable>();
            if (explodable != null && !explodable.HasExploded)
            {
                Debug.Log("Exploding");

                explodable.Explode();
            }
            wettable.SetWetness(0);
        }
    }
}