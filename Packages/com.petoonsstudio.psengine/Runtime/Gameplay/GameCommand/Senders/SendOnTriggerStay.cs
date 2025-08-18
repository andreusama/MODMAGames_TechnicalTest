using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnTriggerStay : SendOnCollision
    {
        void OnTriggerStay(Collider other)
        {
            if (TagAndLayerMatch(other.gameObject))
            {
                Send(other.transform);
            }
        }
    }
}
