using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class ControllerVibrationSignalEmitter : SignalEmitter
    {
        public VibrationPreset Preset;
        public int PlayerIndex;
    }
}