using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class CopyDepthFeature : ScriptableRendererFeature {

    private class SetGlobalTexture : ScriptableRenderPass {

        private RTHandle rt;

        public SetGlobalTexture(RenderPassEvent renderPassEvent) {
            base.renderPassEvent = renderPassEvent;
        }

        public void Setup(RTHandle rt) {
            this.rt = rt;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) { }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.SetGlobalTexture(rt.name, rt.nameID);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

    }

    [Tooltip("Event that the CopyDepth should occur at")]
    public RenderPassEvent _event = RenderPassEvent.AfterRenderingTransparents;
    [Tooltip("Assign the Renderer asset here, I don't know how else to obtain it")]
    public UniversalRendererData rendererAsset;
    [Tooltip("Can toggle this to try to match the depth texture format URP is using, might avoid allocating an extra RTHandle. Compare the target used by DepthPrepass/CopyDepth passes in Frame Debugger")]
    public bool depthUsesPrepass = true;

    private SetGlobalTexture setGlobalTexPass;
    private CopyDepthPass copyDepthPass;
    private Material m_CopyDepthMaterial;
    private RTHandle depthTexture;

#if UNITY_SWITCH || UNITY_ANDROID
    const GraphicsFormat k_DepthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;
    const int k_DepthBufferBits = 24;
#else
    const GraphicsFormat k_DepthStencilFormat = GraphicsFormat.D32_SFloat_S8_UInt;
    const int k_DepthBufferBits = 32;
#endif

    public override void Create() {
        if (m_CopyDepthMaterial == null && rendererAsset != null)
            m_CopyDepthMaterial = CoreUtils.CreateEngineMaterial(rendererAsset.shaders.copyDepthPS);

        setGlobalTexPass = new SetGlobalTexture(_event);
        copyDepthPass = new CopyDepthPass(_event, m_CopyDepthMaterial);
    }

    protected override void Dispose(bool disposing) {
        if (m_CopyDepthMaterial != null) CoreUtils.Destroy(m_CopyDepthMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
        // Allocate Depth Texture
        // based on UniversalRenderer.cs (2022.2/staging)
        // https://github.com/Unity-Technologies/Graphics/blob/866e82896d52796070d57f893e2ea1c4c56d8b95/Packages/com.unity.render-pipelines.universal/Runtime/UniversalRenderer.cs#L788
        var depthDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        if (depthUsesPrepass) {
            depthDescriptor.graphicsFormat = GraphicsFormat.None;
            depthDescriptor.depthStencilFormat = k_DepthStencilFormat;
            depthDescriptor.depthBufferBits = k_DepthBufferBits;
        } else {
            depthDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
            depthDescriptor.depthStencilFormat = GraphicsFormat.None;
            depthDescriptor.depthBufferBits = 0;
        }
        depthDescriptor.msaaSamples = 1;// Depth-Only pass don't use MSAA
        RenderingUtils.ReAllocateIfNeeded(ref depthTexture, depthDescriptor, FilterMode.Point, wrapMode: TextureWrapMode.Clamp, name: "_CameraDepthTexture");

        setGlobalTexPass.Setup(depthTexture);
        copyDepthPass.Setup(renderer.cameraDepthTargetHandle, depthTexture);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (renderingData.cameraData.isPreviewCamera) return;
        if (m_CopyDepthMaterial == null) return;

        renderer.EnqueuePass(setGlobalTexPass);
        renderer.EnqueuePass(copyDepthPass);
    }
}