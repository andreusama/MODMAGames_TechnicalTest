using PetoonsStudio.PSEngine.Gameplay;
using PetoonsStudio.PSEngine.Timeline;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Interaction
{
    abstract public class InteractionSensor : MonoBehaviour, IGameCommandSender, ICutsceneResponder
    {
        [Header("Events")]
        [SerializeField] protected UnityEvent m_OnInteract;

        public IInteractable LastCandidate { get; protected set; }

        public virtual bool IsAvailable { get; protected set; } = true;

        protected virtual void Update()
        {
            if (!IsAvailable)
            {
                RemoveCurrentInteractable();
            }
        }

        /// <summary>
        /// On Disable
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveCurrentInteractable();
        }

        protected void ChangeCurrentInteractable(IInteractable newInteractable)
        {
            if (LastCandidate != null && LastCandidate.GameObject != null)
                SetLastCandidateVisibility(false);

            LastCandidate = newInteractable;

            if (LastCandidate != null && LastCandidate.GameObject != null)
                SetLastCandidateVisibility(true);
        }

        protected void RemoveCurrentInteractable()
        {
            if (LastCandidate == null || LastCandidate.GameObject == null)
                return;

            SetLastCandidateVisibility(false);

            LastCandidate = null;
        }

        protected void SetLastCandidateVisibility(bool visible)
        {
            LastCandidate.SetVisibility(visible);
        }

        /// <summary>
        /// Primary interact input request
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnInteract(InputValue value)
        {
            if (value.isPressed)
                Send(transform);
        }

        public void Send(Transform source)
        {
            if (LastCandidate == null || LastCandidate.GameObject == null)
                return;

            if (!IsAvailable)
                return;

            m_OnInteract?.Invoke();

            LastCandidate.PerformInteraction(transform);
        }

        public void SetCutsceneState()
        {
            IsAvailable = false;
        }

        public void SetGameplayState()
        {
            IsAvailable = true;
        }
    }
}
