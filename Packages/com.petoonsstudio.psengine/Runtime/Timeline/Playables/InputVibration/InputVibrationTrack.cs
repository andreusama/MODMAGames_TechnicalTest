using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackClipType(typeof(InputVibrationClip))]
    [TrackClipType(typeof(AudioInputVibrationClip))]
    public class InputVibrationTrack : TrackAsset
    {
    }
}
