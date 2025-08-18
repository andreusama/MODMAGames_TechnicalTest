using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnTriggerExit : SendOnCollision
    {
        void OnTriggerExit(Collider other)
        {
            if (TagAndLayerMatch(other.gameObject))
            {
                Send(other.transform);
            }
        }
    }
}
