using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisuals : MonoBehaviour
{

    [Header("Tile Resource VisualData")] [SerializeField]
    private ResourceVisualDataSO resourceVisualData;

    private const string TILE_DEFAULT_STATE_NAME = "None";
    private const string TILE_START_HARVEST_TRIGGER = "Start";
    private const string TILE_END_HARVEST_TRIGGER = "End";
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

    private void Start()
    {
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
        GlobalSettings.OnGameSpeedChanged += OnGameSpeedChanged;
        
        OnGameSpeedChanged(GlobalSettings.GameSpeed);
    }

    private void OnDestroy()
    {
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
    }

    private void CollectRenderers()
    {
        _tileRenderers = transform.GetComponentsInChildren<Renderer>();
    }

    public void PopulateResourceVisuals(List<ResourceItem> resources)
    {

    }

    public void StartTileHarvestAnimation(Dictionary<ResourceType, int> harvestChange)
    {
        AnimationUtils.ResetAnimator(animator, TILE_DEFAULT_STATE_NAME);
        animator.SetTrigger(TILE_START_HARVEST_TRIGGER);
    }

    public void TriggerTileHarvestAnimation(Dictionary<ResourceType, int> harvestChange)
    {
        animator.SetTrigger(TILE_SHAKE_ANIMATION_TRIGGER);
    }
    
    public void EndTileHarvestAnimation()
    {
        animator.SetTrigger(TILE_END_HARVEST_TRIGGER);
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

    private void OnGameSpeedChanged(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }
}
