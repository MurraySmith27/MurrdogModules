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
    private const string TILE_APPEAR_ANIMATION_TRIGGER = "Appear";

    [SerializeField] private Animator animator;

    [SerializeField] private Transform buildingsRoot = new RectTransform();
    
    [SerializeField] private Transform citizensRoot = new RectTransform();
    
    [SerializeField] private ShadowOverlayVisuals shadowOverlayVisuals;

    [SerializeField] private ShadowOverlayVisuals grayOutOverlayVisuals;

    [SerializeField] private float bonusTickParticleSystemStartDelay = 0.15f;
    
    [SerializeField] private GameObject bonusTickParticleSystemPrefab;

    [SerializeField] private float bonusTickParticleSystemDuration = 3f;

    private List<ResourceIcon> _instantiatedResourceIcons = new List<ResourceIcon>();

    private Renderer[] _tileRenderers;

    private List<BuildingBehaviour> _attachedBuildings = new();

    private List<CitizenBehaviour> _attachedCitizens = new();

    private bool _playedAppearAnimation = false;

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

    public void StartTileHarvestAnimation()
    {
        AnimationUtils.ResetAnimator(animator, TILE_DEFAULT_STATE_NAME);
        animator.SetTrigger(TILE_START_HARVEST_TRIGGER);
    }

    public void TriggerTileHarvestAnimation()
    {
        animator.SetTrigger(TILE_SHAKE_ANIMATION_TRIGGER);
    }

    public void TriggerBonusTickAnimation()
    {
        OrpheusTiming.InvokeCallbackAfterSecondsGameTime(bonusTickParticleSystemStartDelay, () =>
        {
            GameObject particleSystem = Instantiate(bonusTickParticleSystemPrefab, transform.position,
                Quaternion.identity, null);

            AsyncUtils.InvokeCallbackAfterSeconds(bonusTickParticleSystemDuration, () =>
            {
                Destroy(particleSystem);
            });
        });
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
    
    public void DetachAllBuildings()
    {
        foreach (BuildingBehaviour building in _attachedBuildings)
        {
            building.transform.SetParent(null);
        }
        
        _attachedCitizens.Clear();
    }

    public void AttachCitizen(CitizenBehaviour citizen)
    {
        _attachedCitizens.Add(citizen);
        
        citizen.transform.SetParent(citizensRoot);
    }

    public void DetachAllCitizens()
    {
        foreach (CitizenBehaviour citizen in _attachedCitizens)
        {
            citizen.transform.SetParent(null);
        }
        
        _attachedCitizens.Clear();
    }

    public void ToggleVisuals(bool enabled)
    {
        foreach (Renderer renderer in _tileRenderers)
        {
            renderer.enabled = enabled;
        }
    }

    public void ToggleShadow(bool enabled)
    {
        shadowOverlayVisuals.gameObject.SetActive(enabled);
    }

    public void ShadowAppearAnimation()
    {
        shadowOverlayVisuals.gameObject.SetActive(true);
        shadowOverlayVisuals.OnReappear();
    }

    public void ToggleGrayOut(bool enabled)
    {
        if (!enabled)
        {
            grayOutOverlayVisuals.OnDisappear();
        }
        else
        {
            grayOutOverlayVisuals.gameObject.SetActive(true);
        }
    }

    private void OnGameSpeedChanged(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }

    public void OnTileAppear()
    {
        shadowOverlayVisuals.OnDisappear();

        if (!_playedAppearAnimation)
        {
            _playedAppearAnimation = true;
            animator.SetTrigger(TILE_APPEAR_ANIMATION_TRIGGER);
        }
    }
}
