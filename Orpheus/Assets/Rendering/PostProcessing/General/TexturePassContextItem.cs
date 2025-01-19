using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public class TexturePassContextItem : ContextItem
{

    public TextureHandle textureToTransfer;

    public override void Reset()
    {
        textureToTransfer = TextureHandle.nullHandle;
    }
}
