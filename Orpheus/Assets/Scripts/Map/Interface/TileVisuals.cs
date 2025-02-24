using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisuals : MonoBehaviour
{

    [Header("Tile Resource VisualData")]
    [SerializeField] private ResourceVisualDataSO resourceVisualData;

    [Header("Resource Visuals")] 
    [SerializeField] private Transform resourceIconParent;
    [SerializeField] private ResourceIcon singleResourceVisualPrefab;
    [SerializeField] private ResourceIcon doubleResourceVisualPrefab;
    [SerializeField] private ResourceIcon tripleResourceVisualPrefab;
    
    private const string MAT_TEXTURE_ACCESSOR = "_MainTex";
    
    private List<ResourceIcon> _instantiatedResourceIcons = new List<ResourceIcon>();

    private Renderer[] _tileRenderers;
    private void Awake()
    {
        CollectRenderers();
    }

    private void CollectRenderers()
    {
        _tileRenderers = transform.GetComponentsInChildren<Renderer>();
    }
    
    public void PopulateResourceVisuals(List<ResourceItem> resources)
    {
        //TODO: add special prefabs on top of this for tile resources of different types
        CollectRenderers();
        return;
        foreach (ResourceIcon icon in _instantiatedResourceIcons)
        {
            Destroy(icon.gameObject);
        }
        
        _instantiatedResourceIcons.Clear();
        
        for (int i = 0; i < resources.Count; i++)
        {
            ResourceIcon resourceIconPrefab = null;
            switch (resources[i].Quantity)
            {
                case 1:
                    resourceIconPrefab = singleResourceVisualPrefab;
                    break;
                case 2:
                    resourceIconPrefab = doubleResourceVisualPrefab;
                    break;
                case 3:
                    resourceIconPrefab = tripleResourceVisualPrefab;
                    break;
                default:
                    continue;
            }
            ResourceIcon newResourceIcon = Instantiate(resourceIconPrefab, resourceIconParent);
            
            newResourceIcon.SetIconImage(resourceVisualData.GetSpriteForResourceItem(resources[i].Type));
            
            _instantiatedResourceIcons.Add(newResourceIcon);
        }
    }

    public void ToggleVisuals(bool enabled)
    {
        foreach (Renderer renderer in _tileRenderers)
        {
            renderer.enabled = enabled;
        }
    }
}
