using PetoonsStudio.PSEngine.Input.Feedback;
using UnityEngine;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class SetColorIFMarker : ColorIFMarker, IHandledNotification
    {

        public Color TargetColor;

        public override void OnInitialize(TrackAsset aPent)
        {
            base.OnInitialize(aPent);
            TargetColor = Color.black;
            TargetColor.a = 1f;
        }

        protected override void SingleMode(ColorIFMarker data)
        {
            SetColorIFMarker mData = data as SetColorIFMarker;
            InputFeedbackDispatcher.Instance.SetUserColor(mData.UserIndex, mData.TargetColor);
        }

        protected override void AllMode(ColorIFMarker data)
        {
            SetColorIFMarker mData = data as SetColorIFMarker;
            InputFeedbackDispatcher.Instance.SetDevicesColor(mData.TargetColor);
        }
    }
}
