#if UNITY_6_OR_NEWER

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class CreateGlobalRenderTextureRendererFeature : ScriptableRendererFeature
{
    class CreateGlobalRenderTextureRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private string _textureName;

        private int _globalTextureID;

        private bool _isDepthTexture;

        public void Setup(string textureName, Material material, bool isDepthTexture)
        {
            _globalTextureID = Shader.PropertyToID(textureName);
            _textureName = textureName;
            _material = material;
            _isDepthTexture = isDepthTexture;

            requiresIntermediateTexture = true;
        }

        private class PassData
        {
            internal Material Material;
            internal TextureHandle SourceTexture;
            internal RendererListHandle RendererList;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var lightData = frameData.Get<UniversalLightData>();
            
            var sourceTextureHandle = resourceData.activeColorTexture;
            
            var newGlobalTextureDescriptor = renderGraph.GetTextureDesc(sourceTextureHandle);
            newGlobalTextureDescriptor.name = _textureName;
            newGlobalTextureDescriptor.clearBuffer = false;
            newGlobalTextureDescriptor.msaaSamples = MSAASamples.None;
            newGlobalTextureDescriptor.depthBufferBits = DepthBits.None;
            
            var newGlobalTextureHandle = renderGraph.CreateTexture(newGlobalTextureDescriptor);
            
            using (var builder =
                   renderGraph.AddRasterRenderPass<PassData>($"CreateGlobalRenderTextureRenderPass-{_textureName}",
                       out var passData))
            {
                passData.Material = _material;
                passData.SourceTexture = sourceTextureHandle;
                
                builder.SetRenderAttachment(newGlobalTextureHandle, 0, AccessFlags.Write);
                builder.UseTexture(sourceTextureHandle, AccessFlags.Read);
                
                builder.AllowGlobalStateModification(true);

                builder.SetGlobalTextureAfterPass(newGlobalTextureHandle, _globalTextureID);
                
                //set Drawing Settings, if depth texture
                if (_isDepthTexture)
                {
                    RenderQueueRange renderQueueRange = RenderQueueRange.opaque;
                    FilteringSettings filteringSettings = new FilteringSettings(renderQueueRange, ~0);
                    ShaderTagId shadersToOverride = new ShaderTagId("DepthOnly");
                    DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shadersToOverride,
                        renderingData, cameraData, lightData, cameraData.defaultOpaqueSortFlags);
                    
                    drawSettings.overrideMaterial = _material;
                    var rendererListParameters = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);

                    passData.RendererList = renderGraph.CreateRendererList(rendererListParameters);

                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);
                }

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
                    var cmd = context.cmd;
                    var material = data.Material;
                    var source = data.SourceTexture;
                    
                    Blitter.BlitTexture(cmd, source, new Vector2(1,1), 0, false);
                });
            }
        }
    }

    CreateGlobalRenderTextureRenderPass m_ScriptablePass;
    
    [SerializeField] private Material material;

    [SerializeField] private string renderTextureName; 
    
    [SerializeField] private RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;

    [SerializeField] private bool isDepthTexture = false;
        
    public override void Create()
    {
        m_ScriptablePass = new CreateGlobalRenderTextureRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = passEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (string.IsNullOrEmpty(renderTextureName))
        {
            Debug.LogError($"Error when adding CreateGlobalRenderTextureFeature: Cannot make render texture with name: {renderTextureName}. pass will be skipped");
            return;
        }

        if (material == null)
        {
            Debug.LogError($"Error when adding CreateGlobalRenderTextureFeature: material is null. pass will be skipped");
            return;
        }
        
        m_ScriptablePass.Setup(renderTextureName, material, isDepthTexture);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


#endif