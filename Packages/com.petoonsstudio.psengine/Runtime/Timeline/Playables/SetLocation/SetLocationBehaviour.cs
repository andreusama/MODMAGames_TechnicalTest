using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SetLocationBehaviour : PlayableBehaviour
    {
        public Vector3 position;
        public Vector3 eulerAngles;
    }

}