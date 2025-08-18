using PetoonsStudio.PSEngine.Timeline;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class PlayerBinder : MonoBehaviour, ICutsceneBinder
    {
        [ReadOnly, SerializeField] protected string m_TrackName = "Player";

        public virtual IEnumerator SetTrackBindings(PlayableDirector director)
        {
            var player = PlayerInput.GetPlayerByIndex(0).gameObject;

            TimelineAsset timelineAsset = (TimelineAsset)director.playableAsset;
            var trackAssets = timelineAsset.outputs;

            foreach (var binding in trackAssets)
            {
                TrackAsset track = binding.sourceObject as TrackAsset;

                if (IsPlayerTrack(track, binding))
                    PerformRebinding(director, binding, player);

                if (track is ControlTrack controlTrack)
                    RebindControlTrack(controlTrack, director);
            }

            yield return null;
        }

        protected virtual bool IsPlayerTrack(TrackAsset track, PlayableBinding binding)
        {
            var group = track.GetGroup();

            if (group != null)
                return group.name.StartsWith(m_TrackName);
            else
                return binding.streamName.StartsWith(m_TrackName);
        }

        protected virtual void RebindControlTrack(ControlTrack track, PlayableDirector director)
        {
            foreach (var clip in track.GetClips())
            {
                var asset = (clip.asset as ControlPlayableAsset);

                if (asset.updateDirector)
                {
                    Object assetObj = director.GetReferenceValue(asset.sourceGameObject.exposedName, out bool valid);
                    GameObject gameObject = assetObj as GameObject;

                    if (valid)
                        StartCoroutine(SetTrackBindings(gameObject.GetComponent<PlayableDirector>()));
                }
            }
        }

        protected void PerformRebinding(PlayableDirector director, PlayableBinding binding, GameObject player)
        {
            if (binding.outputTargetType == typeof(SignalReceiver))
                director.SetGenericBinding(binding.sourceObject, player);
            else if (binding.outputTargetType.IsSubclassOf(typeof(Component)))
                director.SetGenericBinding(binding.sourceObject, player.GetComponent(binding.outputTargetType));
            else
                director.SetGenericBinding(binding.sourceObject, player);
        }
    }
}