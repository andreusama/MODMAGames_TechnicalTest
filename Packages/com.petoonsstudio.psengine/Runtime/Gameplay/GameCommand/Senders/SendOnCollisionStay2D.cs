using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnCollisionStay2D : SendOnCollision
    {
        void OnCollisionStay2D(Collision2D collision)
        {
            if (TagAndLayerMatch(collision.gameObject))
            {
                Send(collision.transform);
            }
        }
    }
}
