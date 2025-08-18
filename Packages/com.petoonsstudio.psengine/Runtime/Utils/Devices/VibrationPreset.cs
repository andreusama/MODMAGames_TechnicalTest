using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CreateAssetMenu(fileName = "NewVibrationPreset", menuName = "Petoons Studio/Vibration/Vibration Preset")]
    public class VibrationPreset : ScriptableObject
    {
        [Tooltip("Determines if the vibration will play when the game is paused. If true it will not stop when paused.")]
        public bool StopOnPause = true;

        [Header("Values")]
        public float LowFrequency;
        public float HighFrequency;
        public float Duration;
        public int Priority;
    }
}
