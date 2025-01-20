// // MIT License
//
// // Copyright (c) 2021 NedMakesGames
//
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files(the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions :
//
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
//
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.
//
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
//
// public class DepthNormalsFeature : ScriptableRendererFeature {
//     class RenderPass : ScriptableRenderPass {
//
//         private Material material;
//         private RTHandle destinationHandle;
//         private List<ShaderTagId> shaderTags;
//         private FilteringSettings filteringSettings;
//
//         public RenderPass(Material material) : base() {
//             this.material = material;
//             // This contains a list of shader tags. The renderer will only render objects with
//             // materials containing a shader with at least one tag in this list
//             this.shaderTags = new List<ShaderTagId>() {
//                 new ShaderTagId("DepthOnly"),
//                 //new ShaderTagId("SRPDefaultUnlit"),
//                 //new ShaderTagId("UniversalForward"),
//                 //new ShaderTagId("LightweightForward"),
//             };
//             // Render opaque materials
//             this.filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
//             destinationHandle = RTHandles.Alloc("_DepthNormalsTexture", "_DepthNormalsTexture");
//         }
//
//         // Configure the pass by creating a temporary render texture and
//         // readying it for rendering
//         public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
//             cmd.GetTemporaryRT(Shader.PropertyToID(destinationHandle.name), cameraTextureDescriptor, FilterMode.Point);
//             ConfigureTarget(destinationHandle);
//             ConfigureClear(ClearFlag.All, Color.black);
//         }
//
//         public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
//
//             // Create the draw settings, which configures a new draw call to the GPU
//             var drawSettings = CreateDrawingSettings(shaderTags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
//             // We cant to render all objects using our material
//             drawSettings.overrideMaterial = material;
//             context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
//         }
//
//         public override void FrameCleanup(CommandBuffer cmd) {
//             cmd.ReleaseTemporaryRT(Shader.PropertyToID(destinationHandle.name));
//         }
//     }
//
//     private RenderPass renderPass;
//
//     public override void Create() {
//         // We will use the built-in renderer's depth normals texture shader
//         Material material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
//         this.renderPass = new RenderPass(material);
//         // Render after shadow caster, depth, etc. passes
//         renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
//     }
//
//     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
//         renderer.EnqueuePass(renderPass);
//     }
// }

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class DepthNormalsTextureFeature : ScriptableRendererFeature
{
    class DepthNormalsRenderPass : ScriptableRenderPass
    {
        private const string textureId = "_DepthNormalsTexture";
        
        private Material _material;
        private List<ShaderTagId> _shaderTags;
        private FilteringSettings _filteringSettings;
        
        private int globalTextureID = Shader.PropertyToID(textureId);

        public void Setup(Material material)
        {
            _material = material;

            _shaderTags = new List<ShaderTagId>()
            {
                new ShaderTagId("DepthOnly")
            };
            
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            requiresIntermediateTexture = true;
        }
        
        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
            public TextureHandle source;
        }

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), _material, 0);
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Draw Depth Normals Texture";

            // This adds a raster render pass to the graph, specifying the name and the data type that will be passed to the ExecutePass function.
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                
                RenderTextureDescriptor textureProperties = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
                TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureProperties, "Depth Normals Texture Temp", false);
                
                builder.SetRenderAttachment(destination, 0);
                
                builder.SetGlobalTextureAfterPass(destination, globalTextureID);

                builder.UseTexture(resourceData.activeColorTexture);
                
                passData.source = resourceData.activeColorTexture;
                
                builder.AllowPassCulling(false);
                
                resourceData.cameraColor = destination;
            }
        }
    }

    DepthNormalsRenderPass pass;

    private Material material;
    
    /// <inheritdoc/>
    public override void Create()
    {
        pass = new();
        // Configures where the render pass should be injected.
        pass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        
        material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");

        pass.Setup(material);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
