using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(LocalizationAudioClip))]
    [TrackBindingType(typeof(AudioSource))]
    public class LocalizationAudioTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<LocalizationAudioMixerBehaviour>.Create(graph, inputCount);
        }
    }
}