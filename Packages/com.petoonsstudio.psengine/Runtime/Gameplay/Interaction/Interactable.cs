using CrossSceneReference;
using KBCore.Refs;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Gameplay;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.Interaction
{
    public struct InteractionEvent
    {
        public IInteractable Interactable;

        public InteractionEvent(IInteractable interactable)
        {
            Interactable = interactable;
        }
    }

    public interface IInteractable : IShowable
    {
        public void PerformInteraction(Transform source);

        public bool IsAvailable { get; }

        public GameObject GameObject { get; }
    }

    public abstract class Interactable<T> : GameCommandReceiver, IInteractable
    {
        [Header("Values")]
        [SerializeField] protected LocalizedString m_InteractionName;

        [Header("Persistence")]
        [SerializeField, Self] protected T m_Collider;

        public T Collider { get { return m_Collider; } }

        public GameObject GameObject
        {
            get
            {
                if (this == null)
                    return null;

                if (gameObject == null)
                    return null;

                return gameObject;
            }
        }

        public void PerformInteraction(Transform source)
        {
            OnStartAction(source);
        }

        public override void Execute()
        {
            var sensor = m_InteractingActor.GetComponent<InteractionSensor>();
            sensor.enabled = false;

            var playerInput = m_InteractingActor.GetComponent<PlayerInput>();
            playerInput.DeactivateInput();
        }

        protected override void EndAction()
        {
            var sensor = m_InteractingActor.GetComponent<InteractionSensor>();
            sensor.enabled = true;

            PSEventManager.TriggerEvent(new InteractionEvent(this));

            var playerInput = m_InteractingActor.GetComponent<PlayerInput>();
            playerInput.ActivateInput();

            base.EndAction();
        }

        public abstract void SetVisibility(bool show);
    }
}
