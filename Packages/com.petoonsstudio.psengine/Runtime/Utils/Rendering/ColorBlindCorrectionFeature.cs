using PetoonsStudio.PSEngine.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
namespace PetoonsStudio.PSEngine.Utils
{
    public class ColorBlindCorrectionFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class CustomPassSettings
        {
            public RenderPassEvent renderPassEvent;
            public Material material;
            public ColorBlindnessType blindnessType;
        }
        [SerializeField] private CustomPassSettings m_Settings;
        private ColorBlindCorrectionPass m_CustomPass;
        public ColorBlindnessType BlindnessType
        {
            set
            {
                m_Settings.blindnessType = value;
            }
        }
        public override void Create()
        {
            m_CustomPass = new ColorBlindCorrectionPass(m_Settings);
        }
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            var cameraColorTarget = renderer.cameraColorTargetHandle;
            m_CustomPass.Setup(cameraColorTarget, renderingData);
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_CustomPass);
        }
    }
}