using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class CombinationEffectRendererFeature : ScriptableRendererFeature
{
    class CombinationEffectPass : ScriptableRenderPass
    {
        private const string m_PassName = "CombinationEffectPass";
        private Material m_BlitMaterial;

        public void Setup(Material mat)
        {
            m_BlitMaterial = mat;
            var stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<CombinationPostProcessingVolumeComponent>();
            m_BlitMaterial.SetFloat("_NormalOutlineStrength", customEffect.normalOutlineStrength.value);
            m_BlitMaterial.SetColor("_NormalOutlineColor", customEffect.normalOutlineColor.value);
            requiresIntermediateTexture = true;
        }

        class PassData
        {
            internal TextureHandle source;
            internal TextureHandle destination;
            internal TextureHandle normalOutlineTexture;
            internal Material mat;
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder =
                   renderGraph.AddRasterRenderPass<PassData>("Combination Render Pass", out var passData))
            {
                var stack = VolumeManager.instance.stack;
                var customEffect = stack.GetComponent<CombinationPostProcessingVolumeComponent>();

                if (!customEffect.IsActive())
                    return;

                var resourceData = frameData.Get<UniversalResourceData>();

                if (resourceData.isActiveTargetBackBuffer)
                {
                    Debug.LogError(
                        $"Skipping render pass. CombinationEffectRendererFeature requires an intermediate " +
                        $"ColorTexture, we can't use the BackBuffer as a texture input.");
                    return;
                }

                var source = resourceData.activeColorTexture;

                //add the outline texture to the resource data so we can pull it in the grayscale pass.
                if (frameData.Contains<TexturePassContextItem>())
                {
                    TexturePassContextItem contextItem = frameData.Get<TexturePassContextItem>();
                    
                    RenderTextureDescriptor textureProperties =
                        new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
                    var tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureProperties, "Temporary texture.", false);
                    
                    passData.source = source;
                    passData.destination = tempTexture;
                    passData.normalOutlineTexture = contextItem.textureToTransfer;
                    passData.mat = m_BlitMaterial;
                    
                    builder.UseTexture(source);
                    builder.UseTexture(passData.normalOutlineTexture);
                    
                    contextItem.textureToTransfer = tempTexture;

                    builder.SetRenderAttachment(tempTexture, 0);
                    
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc<PassData>(ExecutePass);
                }
            }
        }

        void ExecutePass(PassData data, RasterGraphContext context)
        {
            data.mat.SetTexture("_NormalOutlineTexture", data.normalOutlineTexture);
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), m_BlitMaterial, 0);
        }
    }

    public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    public Material material;
    
    CombinationEffectPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CombinationEffectPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = injectionPoint;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
        {
            Debug.LogWarning("CombinationEffectRendererFeature material is null and will be skipped.");
            return;
        }
        m_ScriptablePass.Setup(material);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
