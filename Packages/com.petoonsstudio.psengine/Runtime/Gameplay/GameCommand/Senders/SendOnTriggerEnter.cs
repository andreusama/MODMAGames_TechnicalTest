using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendOnTriggerEnter : SendOnCollision
    {
        void OnTriggerEnter(Collider other)
        {
            if (TagAndLayerMatch(other.gameObject))
            {
                Send(other.transform);
            }
        }
    }
}
