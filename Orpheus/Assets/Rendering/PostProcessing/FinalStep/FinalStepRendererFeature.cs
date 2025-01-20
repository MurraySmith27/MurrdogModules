using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class FinalStepEffectRendererFeature : ScriptableRendererFeature
{
    class FinalStepEffectPass : ScriptableRenderPass
    {
        private const string m_PassName = "FinalStepEffectPass";
        public void Setup()
        {
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
                   renderGraph.AddRasterRenderPass<PassData>("FinalStep Render Pass", out var passData))
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                if (resourceData.isActiveTargetBackBuffer)
                {
                    Debug.LogError(
                        $"Skipping render pass. FinalStepEffectRendererFeature requires an intermediate " +
                        $"ColorTexture, we can't use the BackBuffer as a texture input.");
                    return;
                }

                //add the outline texture to the resource data so we can pull it in the grayscale pass.
                if (frameData.Contains<TexturePassContextItem>())
                {
                    TexturePassContextItem contextItem = frameData.Get<TexturePassContextItem>();
                    
                    passData.source = contextItem.textureToTransfer;

                    builder.UseTexture(contextItem.textureToTransfer);
                    
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                    
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc<PassData>(ExecutePass);
                }
            }
        }

        void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), 0, false);
        }
    }

    public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    
    FinalStepEffectPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new FinalStepEffectPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = injectionPoint;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.Setup();
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
