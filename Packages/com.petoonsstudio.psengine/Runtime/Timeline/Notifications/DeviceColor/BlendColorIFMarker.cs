using PetoonsStudio.PSEngine.Input.Feedback;
using UnityEngine;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class BlendColorIFMarker : ColorIFMarker, IHandledNotification
    {
        public Color TargetColor;
        [Min(InputFeedbackAsyncCommand.SECONDS_PER_TICK)]
        public float BlendDuration = 1f;

        public override void OnInitialize(TrackAsset aPent)
        {
            base.OnInitialize(aPent);
            TargetColor = Color.black;
            TargetColor.a = 1f;
        }

        protected override void SingleMode(ColorIFMarker data)
        {
            BlendColorIFMarker mData = data as BlendColorIFMarker;
            InputFeedbackDispatcher.Instance.SetUserColor(mData.UserIndex, mData.TargetColor);
        }

        protected override void AllMode(ColorIFMarker data)
        {
            BlendColorIFMarker mData = data as BlendColorIFMarker;
            InputFeedbackDispatcher.Instance.SetDevicesColor(mData.TargetColor);
        }
    }
}
