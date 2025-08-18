using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Timeline
{
    public interface ICutsceneResponder
    {
        void SetCutsceneState();
        void SetGameplayState();
    }
}