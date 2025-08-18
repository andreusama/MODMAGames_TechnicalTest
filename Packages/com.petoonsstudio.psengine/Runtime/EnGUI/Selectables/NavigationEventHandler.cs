using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine
{
    [RequireComponent(typeof(Selectable))]
    public class NavigationEventHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IMoveHandler
    {
        public UnityEvent OnSelectEvent;
        public UnityEvent OnDeselectEvent;
        public UnityEvent OnMoveSuccessEvent;
        public UnityEvent OnMoveCancelEvent;

        public UnityEvent OnMoveLeftEvent;
        public UnityEvent OnMoveRightEvent;
        public UnityEvent OnMoveDownEvent;
        public UnityEvent OnMoveUpEvent;

        [SerializeField, Self] private Selectable m_Selectable;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            OnDeselectEvent?.Invoke();
        }

        public void OnMove(AxisEventData eventData)
        {
            Selectable nextSelection = null;
            
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    nextSelection = m_Selectable.FindSelectableOnLeft();
                    OnMoveLeftEvent?.Invoke();
                    break;
                case MoveDirection.Right:
                    nextSelection = m_Selectable.FindSelectableOnRight();
                    OnMoveRightEvent?.Invoke();
                    break;
                case MoveDirection.Down:
                    nextSelection = m_Selectable.FindSelectableOnDown();
                    OnMoveDownEvent?.Invoke();
                    break;
                case MoveDirection.Up:
                    nextSelection = m_Selectable.FindSelectableOnUp();
                    OnMoveUpEvent?.Invoke();
                    break;
            }

            if (nextSelection != null && nextSelection != gameObject)
                OnMoveSuccessEvent?.Invoke();
            else
                OnMoveCancelEvent?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectEvent?.Invoke();
        }
    }
}
