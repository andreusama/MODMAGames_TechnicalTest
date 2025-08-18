using PetoonsStudio.PSEngine.Utils;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static PetoonsStudio.PSEngine.Framework.AdvancedGraphicSettings;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace PetoonsStudio.PSEngine.Framework
{
    [Serializable]
    public class GraphicsSettings : IGraphicsSettings
    {
        public enum CustomQualityLevel
        {
            Low = 0,
            Medium = 1,
            High = 2,
            Ultra = 3,
            Custom = 4
        }

        public enum CustomScreenMode
        {
            FullScreen,
            FullScreenWindow,
            Windowed
        }

        [System.Serializable]
        public struct PresetData
        {
            public CustomQualityLevel Level;
            public CustomizableAdvancedGraphicsSettingsSO Data;
        }

        [Header("LOD Bias Values")]
        public float LODHigh = 1f;
        public float LODMedium = 0.75f;
        public float LODLow = 0.5f;

        [Header("Max LOD Level")]
        public int MaxLODLevelHigh = 0;
        public int MaxLODLevelMedium = 1;
        public int MaxLODLevelLow = 2;

        [Header("Particles")]
        public int ParticleBudgetHigh = 1024;
        public int ParticleBudgetMedium = 512;
        public int ParticleBudgetLow = 128;

        [Header("Shadow Resolution")]
        public float ShadowDistanceVeryHigh = 100f;
        public float ShadowDistanceHigh = 75f;
        public float ShadowDistanceMedium = 50f;
        public float ShadowDistanceLow = 25f;
        public int ShadowMainResolution = 2048;
        public int ShadowAdditionResolution = 1024;

        [Header("Shadow Cascasdes")]
        public int ShadowCascadeVeryHigh = 4;
        public int ShadowCascadeHigh = 3;
        public int ShadowCascadeMedium = 2;
        public int ShadowCascadeLow = 1;

        [Header("Shadow Mask")]
        public ShadowmaskMode ShadowMaskModeVeryHigh = ShadowmaskMode.DistanceShadowmask;
        public ShadowmaskMode ShadowMaskModeHigh = ShadowmaskMode.DistanceShadowmask;
        public ShadowmaskMode ShadowMaskModeMedium = ShadowmaskMode.DistanceShadowmask;
        public ShadowmaskMode ShadowMaskModeLow = ShadowmaskMode.DistanceShadowmask;

        [Header("Lighting")]
        public int LightsHigh = 8;
        public int LightsMedium = 4;
        public int LightsLow = 2;

        [Header("Presets")]
        public PresetData[] PresetDataList;

        [Header("Runtime Values")]
        [ReadOnly] public FullScreenMode ScreenMode;
        [ReadOnly] public CustomQualityLevel GraphicsPreset;
        [ReadOnly] public AdvancedGraphicSettings AdvancedGraphicSettings;

        public virtual void ApplySettings()
        {
#if UNITY_STANDALONE
            ApplyScreenMode();
            ApplyPreset();
#endif
        }

        public void ApplyScreenMode()
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, ScreenMode, Screen.currentResolution.refreshRateRatio);
        }

        public void ApplyPreset()
        {
            if (GraphicsPreset != CustomQualityLevel.Custom)
                UpdateAdvancedSettingsFromCurrent();

            ApplyAdvancedGraphicSettings();
        }

        protected virtual CustomizableAdvancedGraphicsSettingsSO UpdateAdvancedSettingsFromCurrent()
        {
            CustomizableAdvancedGraphicsSettingsSO data = null;
            foreach (var dataDuple in PresetDataList)
            {
                if (dataDuple.Level == GraphicsPreset)
                    data = dataDuple.Data;
            }

            if (data == null)
            {
                Debug.LogWarning($"Can't find data for preset {GraphicsPreset}");
                return null;
            }

            AdvancedGraphicSettings.MaxTextureQuality = data.MaxTextureQuality;
            AdvancedGraphicSettings.LoD = data.LoD;
            AdvancedGraphicSettings.ShadowQuality = data.ShadowQuality;
            AdvancedGraphicSettings.ShadowResolution = data.ShadowResolution;
            AdvancedGraphicSettings.VSync = data.VSync;
            AdvancedGraphicSettings.Framerate = data.Framerate;
            AdvancedGraphicSettings.ResolutionScale = data.ResolutionScale;
            AdvancedGraphicSettings.Antialiasing = data.Antialiasing;
            AdvancedGraphicSettings.GIRealtime = data.GIRealtimeUsage;
            AdvancedGraphicSettings.Particles = data.Particles;

            return data;
        }

        public virtual void ApplyAdvancedGraphicSettings()
        {
            ApplyTextureQuality();
            ApplyLoD();
            ApplyShadowQuality();
            ApplyShadowResolution();
            ApplyVSyncFramerate();
            ApplyAA();
            ApplyGlobalIlumination();
            ApplyParticleQuality();
            ApplyResolutionScale();
        }

        public void ApplyTextureQuality()
        {
            bool supportsHDR;
            Downsampling downsampling;
            switch (AdvancedGraphicSettings.MaxTextureQuality)
            {
                case MaxTextureQualityLevel.Quarter:
                    supportsHDR = false;
                    downsampling = Downsampling._4xBilinear;
                    break;
                case MaxTextureQualityLevel.Original:
                    supportsHDR = true;
                    downsampling = Downsampling.None;
                    break;
                case MaxTextureQualityLevel.Half:
                    supportsHDR = true;
                    downsampling = Downsampling._2xBilinear;
                    break;
                case MaxTextureQualityLevel.Eighth:
                    supportsHDR = false;
                    downsampling = Downsampling._4xBilinear;
                    break;
                default:
                    supportsHDR = true;
                    downsampling = Downsampling.None;
                    break;
            }

#if UNITY_2022_3_OR_NEWER
            QualitySettings.globalTextureMipmapLimit = (int)AdvancedGraphicSettings.MaxTextureQuality;
#else
            QualitySettings.masterTextureLimit = (int)AdvancedGraphicSettings.MaxTextureQuality;
#endif
            (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).supportsHDR = supportsHDR;

            var downSampling = typeof(UniversalRenderPipelineAsset).GetField("m_OpaqueDownsampling", BindingFlags.Instance | BindingFlags.NonPublic);
            downSampling.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, downsampling);
        }

        public void ApplyLoD()
        {
            float bias;
            int level;
            switch (AdvancedGraphicSettings.LoD)
            {
                case LoDLevel.Low:
                    bias = LODLow;
                    level = MaxLODLevelLow;
                    break;
                case LoDLevel.Medium:
                    bias = LODMedium;
                    level = MaxLODLevelMedium;
                    break;
                case LoDLevel.High:
                    bias = LODHigh;
                    level = MaxLODLevelHigh;
                    break;
                default:
                    bias = LODHigh;
                    level = MaxLODLevelHigh;
                    break;
            }

            QualitySettings.lodBias = bias;
            QualitySettings.maximumLODLevel = level;
        }

        public void ApplyShadowQuality()
        {
            QualitySettings.shadows = AdvancedGraphicSettings.ShadowQuality;

            var softShadowsQuality = typeof(UniversalRenderPipelineAsset).GetField("m_SoftShadowQuality", BindingFlags.Instance | BindingFlags.NonPublic);

            switch (AdvancedGraphicSettings.ShadowQuality)
            {
                case UnityEngine.ShadowQuality.All:
                    softShadowsQuality.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, SoftShadowQuality.High);
                    break;
                case UnityEngine.ShadowQuality.HardOnly:
                    softShadowsQuality.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, SoftShadowQuality.Medium);
                    break;
                case UnityEngine.ShadowQuality.Disable:
                    softShadowsQuality.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, SoftShadowQuality.Low);
                    break;
            }
        }

        public void ApplyShadowResolution()
        {
            QualitySettings.shadowResolution = AdvancedGraphicSettings.ShadowResolution;

            int shadowCascade;
            float shadowDistance;
            int mainResolution;
            int additionalResolution;
            ShadowmaskMode shadowMaskMode;

            switch (AdvancedGraphicSettings.ShadowResolution)
            {
                case UnityEngine.ShadowResolution.VeryHigh:
                    shadowDistance = ShadowDistanceVeryHigh;
                    shadowCascade = ShadowCascadeVeryHigh;
                    mainResolution = ShadowMainResolution;
                    additionalResolution = ShadowAdditionResolution;
                    shadowMaskMode = ShadowMaskModeVeryHigh;
                    break;
                case UnityEngine.ShadowResolution.High:
                    shadowDistance = ShadowDistanceHigh;
                    shadowCascade = ShadowCascadeHigh;
                    mainResolution = ShadowMainResolution;
                    additionalResolution = ShadowAdditionResolution;
                    shadowMaskMode = ShadowMaskModeHigh;
                    break;
                case UnityEngine.ShadowResolution.Medium:
                    shadowDistance = ShadowDistanceMedium;
                    shadowCascade = ShadowCascadeMedium;
                    mainResolution = ShadowMainResolution / 2;
                    additionalResolution = ShadowAdditionResolution / 2;
                    shadowMaskMode = ShadowMaskModeMedium;
                    break;
                case UnityEngine.ShadowResolution.Low:
                    shadowDistance = ShadowDistanceLow;
                    shadowCascade = ShadowCascadeLow;
                    mainResolution = ShadowMainResolution / 4;
                    additionalResolution = ShadowAdditionResolution / 4;
                    shadowMaskMode = ShadowMaskModeLow;
                    break;
                default:
                    shadowDistance = ShadowDistanceVeryHigh;
                    shadowCascade = ShadowCascadeVeryHigh;
                    mainResolution = ShadowMainResolution;
                    additionalResolution = ShadowAdditionResolution;
                    shadowMaskMode = ShadowMaskModeVeryHigh;
                    break;
            }

            QualitySettings.shadowmaskMode = shadowMaskMode;

            (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).shadowCascadeCount = shadowCascade;
            (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).shadowDistance = shadowDistance;

            var mainLightShadowmapResolutionFieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_MainLightShadowmapResolution", BindingFlags.Instance | BindingFlags.NonPublic);
            var additionalLightShadowmapResolutionFieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_AdditionalLightsShadowmapResolution", BindingFlags.Instance | BindingFlags.NonPublic);
            var additionalLightCookieResolutionFieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_AdditionalLightsCookieResolution", BindingFlags.Instance | BindingFlags.NonPublic);

            mainLightShadowmapResolutionFieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, mainResolution);
            additionalLightShadowmapResolutionFieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, additionalResolution);
            additionalLightCookieResolutionFieldInfo.SetValue(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, additionalResolution);
        }

        public void ApplyVSyncFramerate()
        {
            QualitySettings.vSyncCount = AdvancedGraphicSettings.VSync ? 1 : 0;
            Application.targetFrameRate = AdvancedGraphicSettings.Framerate;

            // If Vsync is enabled and framerate is fixed we should prioritize the most capped framerate
            if (Application.targetFrameRate != -1 && QualitySettings.vSyncCount != 0)
            {
                if (Application.targetFrameRate < Screen.currentResolution.refreshRateRatio.value)
                    QualitySettings.vSyncCount = 0;
            }
        }

        public void ApplyAA()
        {
            (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).msaaSampleCount = (int)AdvancedGraphicSettings.Antialiasing;
            QualitySettings.antiAliasing = (int)AdvancedGraphicSettings.Antialiasing;
        }

        public void ApplyGlobalIlumination()
        {
            int count;
            switch (AdvancedGraphicSettings.GIRealtime)
            {
                case UnityEngine.Rendering.RealtimeGICPUUsage.High:
                    count = LightsHigh;
                    break;
                case UnityEngine.Rendering.RealtimeGICPUUsage.Low:
                    count = LightsLow;
                    break;
                case UnityEngine.Rendering.RealtimeGICPUUsage.Medium:
                    count = LightsMedium;
                    break;
                case UnityEngine.Rendering.RealtimeGICPUUsage.Unlimited:
                    count = LightsHigh;
                    break;
                default:
                    count = LightsHigh;
                    break;
            }

            (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).maxAdditionalLightsCount = count;

            QualitySettings.realtimeGICPUUsage = (int)AdvancedGraphicSettings.GIRealtime;
        }

        public void ApplyParticleQuality()
        {
            switch (AdvancedGraphicSettings.Particles)
            {
                case ParticleQuality.High:
                    QualitySettings.particleRaycastBudget = ParticleBudgetHigh;
                    break;
                case ParticleQuality.Medium:
                    QualitySettings.particleRaycastBudget = ParticleBudgetMedium;
                    break;
                case ParticleQuality.Low:
                    QualitySettings.particleRaycastBudget = ParticleBudgetLow;
                    break;
            }
        }

        public void ApplyResolutionScale()
        {
            (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).renderScale = AdvancedGraphicSettings.ResolutionScale;
        }
    }

    public interface IGraphicsSettings
    {
        public void ApplySettings();
    }

    [System.Serializable]
    public class SerializableGraphicsGameSettings
    {
        public FullScreenMode ScreenMode;
        public GraphicsSettings.CustomQualityLevel GraphicsPreset;
        public AdvancedGraphicSettings AdvancedGraphicSettings;

        public SerializableGraphicsGameSettings()
        {

        }

        public SerializableGraphicsGameSettings(GraphicsSettings settings)
        {
            ScreenMode = settings.ScreenMode;
            GraphicsPreset = settings.GraphicsPreset;
            AdvancedGraphicSettings = settings.AdvancedGraphicSettings;
        }

        public virtual void Deserialize(GraphicsSettings settings)
        {
            settings.ScreenMode = ScreenMode;
            settings.GraphicsPreset = GraphicsPreset;
            settings.AdvancedGraphicSettings = AdvancedGraphicSettings;
        }
    }
}