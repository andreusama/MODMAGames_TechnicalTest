using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnCollisionExit : SendOnCollision
    {
        void OnCollisionExit(Collision collision)
        {
            if (TagAndLayerMatch(collision.gameObject))
            {
                Send(collision.transform);
            }
        }
    }

}
