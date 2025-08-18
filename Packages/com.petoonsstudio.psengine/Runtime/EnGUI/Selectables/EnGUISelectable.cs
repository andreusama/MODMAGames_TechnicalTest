using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class EnGUISelectable : Selectable, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        protected MMF_Player m_OnSelectFeedback;
        public UnityEvent OnSelectEvent;

        [SerializeField]
        protected MMF_Player m_OnDeselectFeedback;
        public UnityEvent OnDeselectEvent;

        public override void OnDeselect(BaseEventData eventData)
        {
            if (m_OnSelectFeedback != null)
                m_OnDeselectFeedback.PlayFeedbacks();

            OnDeselectEvent?.Invoke();
            base.OnDeselect(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if(m_OnDeselectFeedback != null)
                m_OnSelectFeedback.PlayFeedbacks();

            OnSelectEvent?.Invoke();
            base.OnSelect(eventData);
        }
    }
}
