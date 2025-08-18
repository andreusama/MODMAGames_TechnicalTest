using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PetoonsStudio.PSEngine.Framework
{
    [Serializable]
    public class AdvancedGraphicSettings
    {
        public enum AntialiasingLevel
        {
            X0 = 1,
            X2 = 2,
            X4 = 4,
            X8 = 8
        }

        public enum LoDLevel
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public enum MaxTextureQualityLevel
        {
            Original = 0,
            Half = 1,
            Quarter = 2,
            Eighth = 3
        }

        public enum ParticleQuality
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public MaxTextureQualityLevel MaxTextureQuality = MaxTextureQualityLevel.Original;
        public LoDLevel LoD = LoDLevel.High;
        public ShadowQuality ShadowQuality = ShadowQuality.All;
        public ShadowResolution ShadowResolution = ShadowResolution.High;
        public bool VSync;
        public int Framerate = -1;
        [Range(0.1f, 1.5f)] public float ResolutionScale = 1.0f;
        public AntialiasingLevel Antialiasing = AntialiasingLevel.X2;
        public RealtimeGICPUUsage GIRealtime = RealtimeGICPUUsage.Unlimited;
        public ParticleQuality Particles = ParticleQuality.High;

    }
}