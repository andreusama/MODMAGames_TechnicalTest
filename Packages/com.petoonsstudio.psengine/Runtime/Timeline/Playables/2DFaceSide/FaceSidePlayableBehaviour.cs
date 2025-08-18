using System;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class FaceSidePlayableBehaviour : PlayableBehaviour
    {
        public FaceSideTrack.PostPlaybackState FacingDirection;
    }
}