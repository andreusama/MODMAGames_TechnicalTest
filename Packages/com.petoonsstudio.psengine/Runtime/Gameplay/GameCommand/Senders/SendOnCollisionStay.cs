using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnCollisionStay : SendOnCollision
    {
        void OnCollisionStay(Collision collision)
        {
            if (TagAndLayerMatch(collision.gameObject))
            {
                Send(collision.transform);
            }
        }
    }
}
