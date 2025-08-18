using UnityEngine;

namespace PetoonsStudio.PSEngine.Interaction
{
    [RequireComponent(typeof(Collider))]
    abstract public class Interactable3D : Interactable<Collider>
    {
        [Header("Restrictions")]
        [SerializeField] protected bool m_MatchForward;
        [SerializeField] protected bool m_CheckBlocking = true;

        public bool MatchForward => m_MatchForward;
        public bool CheckBlocking => m_CheckBlocking;
    }
}
