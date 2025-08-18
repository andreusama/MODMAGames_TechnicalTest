using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public interface ICutsceneBinder
    {
        public IEnumerator SetTrackBindings(PlayableDirector director);
    }
}