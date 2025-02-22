using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{

    [Header("Tile Resource VisualData")]
    [SerializeField] private ResourceVisualDataSO resourceVisualData;

    [Header("Tile Objects")] 
    [SerializeField] private Renderer[] tileResourceIconMeshRenderers;


    private const string MAT_TEXTURE_ACCESSOR = "_MainTex";
    
    public void PopulateResourceVisuals(List<ResourceItem> resources)
    {
        if (resources.Count > tileResourceIconMeshRenderers.Length)
        {
            Debug.LogError("Too many resources on tile! maybe there are too many types populated in the MapResourcesGenerator");
            return;
        }

        for (int i = 0; i < resources.Count; i++)
        {
            OverrideRendererMainTexture(tileResourceIconMeshRenderers[i], resourceVisualData.GetTextureForResourceItem(resources[i]));
        }
    }

    private void OverrideRendererMainTexture(Renderer renderer, Texture2D tex)
    {
        MaterialPropertyBlock block = new();
        renderer.GetPropertyBlock(block);
        
        block.SetTexture(MAT_TEXTURE_ACCESSOR, tex);
        
        renderer.SetPropertyBlock(block);
    }
}
