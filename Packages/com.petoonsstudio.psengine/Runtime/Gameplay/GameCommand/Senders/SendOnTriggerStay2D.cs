using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnTriggerStay2D : SendOnCollision
    {
        void OnTriggerStay2D(Collider2D other)
        {
            if (TagAndLayerMatch(other.gameObject))
            {
                Send(other.transform);
            }
        }
    }
}
