using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static PetoonsStudio.PSEngine.Framework.AdvancedGraphicSettings;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "CustomizableSettingsSO", menuName = "Petoons Studio/PSEngine/Framework/Settings/Customizable Advanced Graphics Settings SO")]
    public class CustomizableAdvancedGraphicsSettingsSO : ScriptableObject
    {
        public MaxTextureQualityLevel MaxTextureQuality;
        public LoDLevel LoD = LoDLevel.High;
        public ShadowQuality ShadowQuality = ShadowQuality.All;
        public ShadowResolution ShadowResolution = ShadowResolution.High;
        public bool VSync;
        public int Framerate = -1;
        [Range(0.1f, 1.5f)] public float ResolutionScale = 1f;
        public AntialiasingLevel Antialiasing = AntialiasingLevel.X2;
        public RealtimeGICPUUsage GIRealtimeUsage = RealtimeGICPUUsage.Unlimited;
        public ParticleQuality Particles = ParticleQuality.High;
    }
}
