using PetoonsStudio.PSEngine.Input.Feedback;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public abstract partial class ColorIFMarker : Marker, IHandledNotification
    {
        public enum TargetMode
        {
            Single,
            All
        }

        public TargetMode Target = TargetMode.All;
        [SerializeField, HideInInspector]
        private bool m_SingleMode = false;
        [ConditionalHide("m_SingleMode")]
        public int UserIndex = -1;

        public PropertyName id { get; }

        public void Resolve(Playable origin, INotification notification, object context)
        {
            if (!InputFeedbackDispatcher.InstanceExists)
                return;

            var inputFeedbackNotification = notification as ColorIFMarker;

            switch (inputFeedbackNotification.Target)
            {
                case TargetMode.Single:
                    SingleMode(inputFeedbackNotification);
                    break;
                case TargetMode.All:
                    AllMode(inputFeedbackNotification);
                    break;
                default:
                    break;
            }
        }

        protected abstract void SingleMode(ColorIFMarker data);
        protected abstract void AllMode(ColorIFMarker data);

#if UNITY_EDITOR
        private void OnValidate()
        {  
            m_SingleMode = Target == TargetMode.Single;
            UserIndex = Target != TargetMode.Single ? -1 : UserIndex;
        }
#endif
    }
}
