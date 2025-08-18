using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class EnGUIButton : EnGUISelectable, ISubmitHandler
    {
        [SerializeField]
        protected MMF_Player m_OnSubmitFeedback;
        public UnityEvent OnSubmitEvent;

        public void OnSubmit(BaseEventData eventData)
        {
            if(m_OnSubmitFeedback != null)
                m_OnSubmitFeedback.PlayFeedbacks();

            OnSubmitEvent?.Invoke();
        }
    }
}
