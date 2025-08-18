using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Interaction
{
    public class InteractionSensor3D : InteractionSensor
    {
        [SerializeField] protected float m_CenterOffset = 1f;

        [Header("Proximity Sensor")]
        [SerializeField] protected bool m_ProximityActive = true;
        [SerializeField] protected float m_ProximityRadius = 2f;

        [Header("Raycast Sensor")]
        [SerializeField] protected bool m_RaycastActive = true;
        [SerializeField] protected float m_DetectionMaxDistance = 1f;
        [SerializeField] protected float m_RaycastRadius = 2f;

        [Header("Layers")]
        [SerializeField] protected LayerMask m_InteractableMask;
        [SerializeField] protected LayerMask m_BlockerMask;

        protected Collider[] m_OverlapResults = new Collider[10];
        protected RaycastHit[] m_DistanceResults = new RaycastHit[5];
        protected Interactable3D m_NewCandidate;

        public Vector3 SensorCenter => transform.position + Vector3.up * m_CenterOffset;

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            m_NewCandidate = null;

            if (!IsAvailable)
            {
                if (LastCandidate != null)
                    RemoveCurrentInteractable();
                return;
            }

            PerformCheckDetection();

            if (LastCandidate == null)
            {
                m_NewCandidate = SelectNewCandidate();
                if (m_NewCandidate != null)
                    ChangeCurrentInteractable(m_NewCandidate);
            }
            else
            {
                var isValid = ValidateCurrentCandidate();
                if (!isValid)
                {
                    m_NewCandidate = SelectNewCandidate();
                    if (m_NewCandidate != null)
                        ChangeCurrentInteractable(m_NewCandidate);
                    else
                        RemoveCurrentInteractable();
                }
            }
        }

        private void PerformCheckDetection()
        {
            if (m_ProximityActive)
                DetectOverlappingInteractions();
            if (m_RaycastActive)
                DetectDistanceInteractions();
        }

        protected Interactable3D SelectNewCandidate()
        {
            foreach (var collider in m_OverlapResults)
            {
                if (collider != null && IsCandidate(collider, out Interactable3D interactable))
                    return interactable;
            }

            foreach (var collider in m_DistanceResults)
            {
                if (collider.collider != null && IsCandidate(collider.collider, out Interactable3D interactable))
                    return interactable;
            }

            return null;
        }

        protected bool ValidateCurrentCandidate()
        {
            foreach (var collider in m_OverlapResults)
            {
                if (collider != null && (LastCandidate as Interactable3D).Collider == collider && IsCandidate(collider, out _))
                    return true;
            }

            foreach (var collider in m_DistanceResults)
            {
                if (collider.collider != null && (LastCandidate as Interactable3D).Collider == collider.collider && IsCandidate(collider.collider, out _))
                    return true;
            }

            return false;
        }

        protected void DetectOverlappingInteractions()
        {
            Array.Clear(m_OverlapResults, 0, m_OverlapResults.Length);
            Physics.OverlapSphereNonAlloc(SensorCenter, m_ProximityRadius, m_OverlapResults, m_InteractableMask, QueryTriggerInteraction.Collide);
        }

        protected void DetectDistanceInteractions()
        {
            Array.Clear(m_DistanceResults, 0, m_DistanceResults.Length);
            Physics.SphereCastNonAlloc(SensorCenter, m_RaycastRadius, transform.forward, m_DistanceResults, m_DetectionMaxDistance, m_InteractableMask, QueryTriggerInteraction.Collide);
        }

        protected bool IsCandidate(Collider item, out Interactable3D interactable)
        {
            if (!item.TryGetComponent(out interactable) || !interactable.IsAvailable)
                return false;

            if (interactable.CheckBlocking && !IsVisible(item))
                return false;

            if (interactable.MatchForward && !IsMatchingDirection(item))
                return false;

            return true;
        }

        protected bool IsVisible(Collider item)
        {
            Vector3 direction = item.bounds.center - SensorCenter;

            if (item.bounds.SqrDistance(transform.position) < m_RaycastRadius)
            {
                return true;
            }
            else if (Physics.Raycast(SensorCenter, direction.normalized, out RaycastHit hit, Mathf.Abs(direction.magnitude), m_BlockerMask | m_InteractableMask))
            {
                if (hit.collider != item)
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        protected bool IsMatchingDirection(Collider item)
        {
            Vector3 direction = item.bounds.center - SensorCenter;

            direction.y = transform.forward.y;
            direction.Normalize();

            return Vector3.Dot(direction, transform.forward) > 0.5f;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(SensorCenter, m_ProximityRadius);
            if (LastCandidate != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(((Interactable3D)LastCandidate).transform.position, 1f);
            }
        }
    }
}