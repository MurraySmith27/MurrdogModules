using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class BlitMaterialRendererFeature : ScriptableRendererFeature
{
    class BlitMaterialRenderPass : ScriptableRenderPass
    {
        private Material _material;

        public void Setup(Material material)
        {
            _material = material;
            requiresIntermediateTexture = true;
        }
        

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            string passName = _material.name;

            var resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogError("Skipping render pass. BlitMaterialRendererFeature requires an intermediate ColorTexture, " +
                               "we can't use the back buffer as a texture input.");
                return;
            }

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{passName}";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, _material, 0);
            renderGraph.AddBlitPass(para, passName: passName);
            
            resourceData.cameraColor = destination;
        }
    }

    BlitMaterialRenderPass m_ScriptablePass;

    public Material m_BlitMaterial;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new BlitMaterialRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_BlitMaterial == null)
        {
            Debug.LogError("No material assigned to BlitMaterialRendererFeature. The pass will not run.");
            return;
        }
        
        m_ScriptablePass.Setup(m_BlitMaterial);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
