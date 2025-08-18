using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SortingLayerGroupClip : PlayableAsset, ITimelineClipAsset
    {
        public SortingLayerGroupPlayableBehaviour template = new SortingLayerGroupPlayableBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SortingLayerGroupPlayableBehaviour>.Create(graph, template);

            return playable;
        }
    }
}