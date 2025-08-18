using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnTriggerExit2D : SendOnCollision
    {
        void OnTriggerExit2D(Collider2D other)
        {
            if (TagAndLayerMatch(other.gameObject))
            {
                Send(other.transform);
            }
        }
    }
}
