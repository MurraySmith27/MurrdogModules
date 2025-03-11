using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisuals : MonoBehaviour
{

    [Header("Tile Resource VisualData")]
    [SerializeField] private ResourceVisualDataSO resourceVisualData;

    private const string TILE_SHAKE_ANIMATION_TRIGGER = "Shake";
    
    [SerializeField] private Animator animator;

    [SerializeField] private Transform buildingsRoot = new RectTransform();
    
    private List<ResourceIcon> _instantiatedResourceIcons = new List<ResourceIcon>();

    private Renderer[] _tileRenderers;

    private List<BuildingBehaviour> _attachedBuildings = new(); 
    
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
        
    }

    public void TriggerTileHarvestAnimation(Dictionary<ResourceType, int> harvestChange)
    {
        animator.SetTrigger(TILE_SHAKE_ANIMATION_TRIGGER);
    }

    public void AttachBuilding(BuildingBehaviour building)
    {
        _attachedBuildings.Add(building);
        
        building.transform.SetParent(buildingsRoot);
    }

    public void ToggleVisuals(bool enabled)
    {
        foreach (Renderer renderer in _tileRenderers)
        {
            renderer.enabled = enabled;
        }
    }
}
