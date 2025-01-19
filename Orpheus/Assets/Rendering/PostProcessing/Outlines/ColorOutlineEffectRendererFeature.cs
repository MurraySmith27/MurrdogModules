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

        class PassData
        {
            internal TextureHandle source;
        }
        
        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder =
                   renderGraph.AddRasterRenderPass<PassData>("Color Outline Render Pass", out var passData))
            {
                var stack = VolumeManager.instance.stack;
                var customEffect = stack.GetComponent<ColorOutlinePostProcessingVolumeComponent>();

                if (!customEffect.IsActive())
                    return;

                var resourceData = frameData.Get<UniversalResourceData>();

                if (resourceData.isActiveTargetBackBuffer)
                {
                    Debug.LogError(
                        $"Skipping render pass. ColorOutlineEffectRendererFeature requires an intermediate " +
                        $"ColorTexture, we can't use the BackBuffer as a texture input.");
                    return;
                }
                
                var source = resourceData.activeColorTexture;
                
                
                TexturePassContextItem contextItem = frameData.Create<TexturePassContextItem>();
                
                RenderTextureDescriptor textureProperties =
                    new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
                contextItem.textureToTransfer = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureProperties, "Outline Buffer Texture", false);
                
                builder.SetRenderAttachment(contextItem.textureToTransfer, 0, AccessFlags.Write);

                builder.AllowPassCulling(false);
                
                passData.source = source;
                
                builder.UseTexture(source);
                
                builder.SetRenderFunc<PassData>(ExecutePass);
            }
        }

        void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), m_BlitMaterial, 0);
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
