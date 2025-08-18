using KBCore.Refs;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    [RequireComponent(typeof(BoxCollider))]
    public class SendOnTriggerExitDirectional : SendOnCollision
    {
        [SerializeField] private Vector3 m_DirectionOfTraversal = Vector3.right;

        [SerializeField, Self] private BoxCollider m_Trigger;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        /// <summary>
        /// Checks whether the player exited the trigger in the allowed direction.
        /// </summary>
        private bool IsTraversingInAllowedDirection(Collider player, Vector3 directionOfTraversal)
        {
            Vector3 centerPoint = m_Trigger.center;
            Vector3 exitPoint = m_Trigger.ClosestPoint(player.transform.position);

            Vector3 direction = exitPoint - centerPoint;
            return Vector3.Dot(direction.normalized, directionOfTraversal.normalized) > 0;
        }

        private void OnTriggerExit(Collider other)
        {
            if (TagAndLayerMatch(other.gameObject) && IsTraversingInAllowedDirection(other, m_DirectionOfTraversal))
            {
                Send(other.transform);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Debug information, showing the direction of traversal that this section allows.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!m_Trigger)
                m_Trigger = GetComponent<BoxCollider>();

            var previousColor = Handles.color;
            Handles.color = Color.black;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(m_DirectionOfTraversal), 1.5f, EventType.Repaint);
            Handles.color = previousColor;
        }
#endif
    }
}
