using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class CutsceneScriptControlBehaviour : PlayableBehaviour
    {
        public bool playerInputEnabled;
        public bool useRootMotion;
        public PlayerInput playerInput;

        public override void OnGraphStart(Playable playable)
        {

        }
    } 
}
