using System;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SkipSectionBehaviour : PlayableBehaviour
    {
        public double SkipToSecond;
    }
}