using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Magnet : MonoBehaviour
    {
        [SerializeField] private float m_OffsetDistance = 0.1f;

        public Transform Target;
        public float Distance => m_OffsetDistance;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Magnetable magnetable) && !magnetable.IsMagnetized)
                magnetable.Magnetize(this);
        }
    }
}