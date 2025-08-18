using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
namespace PetoonsStudio.PSEngine
{
    public class ColorBlindCorrectionPass : ScriptableRenderPass
    {
        RenderTargetIdentifier m_Source;
        RTHandle m_Destination;
        Material m_MaterialToBlit;
        ColorBlindnessType m_BlindnessType;
        public ColorBlindCorrectionPass(ColorBlindCorrectionFeature.CustomPassSettings settings)
        {
            renderPassEvent = settings.renderPassEvent;
            m_MaterialToBlit = settings.material;
            m_BlindnessType = settings.blindnessType;
        }
        public Material MaterialToBlit => m_MaterialToBlit;
        // This isn't part of the ScriptableRenderPass class and is our own addition.
        // For this custom pass we need the camera's color target, so that gets passed in.
        public void Setup(RenderTargetIdentifier cameraColorTargetIdent, RenderingData renderingData)
        {
            m_Source = cameraColorTargetIdent;
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref m_Destination, desc, UnityEngine.FilterMode.Point, TextureWrapMode.Clamp, name: "_ColorBlindPassHandle");
            switch (m_BlindnessType)
            {
                case ColorBlindnessType.Deuteranopia:
                    m_MaterialToBlit.EnableKeyword("_MODE_DEUTERANOPIA");
                    m_MaterialToBlit.DisableKeyword("_MODE_TRITANOPIA");
                    m_MaterialToBlit.DisableKeyword("_MODE_PROTANOPIA");
                    break;
                case ColorBlindnessType.Tritanopia:
                    m_MaterialToBlit.DisableKeyword("_MODE_DEUTERANOPIA");
                    m_MaterialToBlit.EnableKeyword("_MODE_TRITANOPIA");
                    m_MaterialToBlit.DisableKeyword("_MODE_PROTANOPIA");
                    break;
                case ColorBlindnessType.Protanopia:
                    m_MaterialToBlit.DisableKeyword("_MODE_DEUTERANOPIA");
                    m_MaterialToBlit.DisableKeyword("_MODE_TRITANOPIA");
                    m_MaterialToBlit.EnableKeyword("_MODE_PROTANOPIA");
                    break;
            }
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(Shader.PropertyToID(m_Destination.name), cameraTextureDescriptor);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(name: "Color Blind");
            cmd.Clear();
            // the actual content of our custom render pass!
            // we apply our material while blitting to a temporary texture
            cmd.Blit(m_Source, m_Destination.nameID, m_MaterialToBlit, 0);
            // ...then blit it back again
            cmd.Blit(m_Destination.nameID, m_Source);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(m_Destination.name));
        }
    }
}