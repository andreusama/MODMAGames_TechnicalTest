using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnCollisionEnter : SendOnCollision
    {
        void OnCollisionEnter(Collision collision)
        {
            if (TagAndLayerMatch(collision.gameObject))
            {
                Send(collision.transform);
            }
        }
    }
}