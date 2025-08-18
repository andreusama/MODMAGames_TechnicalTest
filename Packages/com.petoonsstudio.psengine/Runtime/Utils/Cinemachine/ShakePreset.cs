using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CreateAssetMenu(fileName = "ShakePreset", menuName = "Petoons Studio/PSEngine/Utils/Cinemachine/Shake Preset")]
    public class ShakePreset : ScriptableObject
    {
        public float Amplitude;
        public float Frequency;
        public float Duration;
        public int Priority;
        public NoiseSettings NoiseSettings;
    }
}