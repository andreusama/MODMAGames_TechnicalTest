using PetoonsStudio.PSEngine.Input.Feedback;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class BreathColorsIFMarker : ColorIFMarker, IHandledNotification
    {
        public List<Color> TargetColors;
        [Min(InputFeedbackAsyncCommand.SECONDS_PER_TICK)]
        public float BlendDuration = 1f;
        [Min(InputFeedbackAsyncCommand.SECONDS_PER_TICK)]
        public float BreathDelay = 1f;
        [Tooltip("Infinite if set to less than 0")]
        public float BreathDuration = -1f;

        public override void OnInitialize(TrackAsset aPent)
        {
            base.OnInitialize(aPent);
            TargetColors = new List<Color>();
            Color c = Color.black;
            c.a = 1f;
            TargetColors.Add(c);
        }

        protected override void SingleMode(ColorIFMarker data)
        {
            BreathColorsIFMarker mData = data as BreathColorsIFMarker;
            InputFeedbackDispatcher.Instance.BreathUserColors(mData.UserIndex, mData.TargetColors,mData.BlendDuration, mData.BreathDelay, mData.BreathDuration);
        }

        protected override void AllMode(ColorIFMarker data)
        {
            BreathColorsIFMarker mData = data as BreathColorsIFMarker;
            InputFeedbackDispatcher.Instance.BreathDevicesColor(mData.TargetColors, mData.BlendDuration, mData.BreathDelay, mData.BreathDuration);
        }
    }
}
