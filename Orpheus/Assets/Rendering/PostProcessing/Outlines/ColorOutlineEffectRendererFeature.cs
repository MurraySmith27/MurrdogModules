using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class ColorOutlineEffectRendererFeature : ScriptableRendererFeature
{
    class ColorOutlineEffectPass : ScriptableRenderPass
    {
        private const string m_PassName = "ColorOutlineEffectPass";
        private Material m_BlitMaterial;

        public void Setup(Material mat)
        {
            m_BlitMaterial = mat;
            var stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<ColorOutlinePostProcessingVolumeComponent>();
            m_BlitMaterial.SetFloat("_ColorThreshold", customEffect.colorThreshold.value);
            m_BlitMaterial.SetFloat("_ColorStrength", customEffect.colorStrength.value);
            m_BlitMaterial.SetFloat("_NormalThreshold", customEffect.normalThreshold.value);
            m_BlitMaterial.SetFloat("_NormalStrength", customEffect.normalStrength.value);
            m_BlitMaterial.SetColor("_Color", customEffect.color.value);
            requiresIntermediateTexture = true;
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<ColorOutlinePostProcessingVolumeComponent>();

            if (!customEffect.IsActive())
                return;
            
            var resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogError($"Skipping render pass. ColorOutlineEffectRendererFeature requires an intermediate " +
                               $"ColorTexture, we can't use the BackBuffer as a texture input.");
                return;
            }

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{m_PassName}";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, m_BlitMaterial, 0);
            renderGraph.AddBlitPass(para, passName: m_PassName);

            resourceData.cameraColor = destination;
        }
    }

    public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    public Material material;
    
    ColorOutlineEffectPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new ColorOutlineEffectPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = injectionPoint;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
        {
            Debug.LogWarning("ColorOutlineEffectRendererFeature material is null and will be skipped.");
            return;
        }
        m_ScriptablePass.Setup(material);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
