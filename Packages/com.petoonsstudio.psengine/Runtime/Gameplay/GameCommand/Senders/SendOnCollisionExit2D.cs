using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnCollisionExit2D : SendOnCollision
    {
        void OnCollisionExit2D(Collision2D collision)
        {
            if (TagAndLayerMatch(collision.gameObject))
            {
                Send(collision.transform);
            }
        }
    }
}
