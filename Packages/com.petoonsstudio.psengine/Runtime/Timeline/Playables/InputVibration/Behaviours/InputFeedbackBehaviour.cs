using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [System.Serializable]
    public partial class InputFeedbackBehaviour : PlayableBehaviour
    {
        protected enum TargetMode
        {
            Single,
            All
        }

        [SerializeField, NotKeyable]
        [Tooltip("If true will let select a user, if false will trigger for all connected devices")]
        protected bool m_SingleMode = false;
        [ConditionalHide("m_SingleMode"), SerializeField, NotKeyable]
        protected int m_UserIndex = 0;
    }
}
