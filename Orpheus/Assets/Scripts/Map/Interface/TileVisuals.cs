using System;
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
    private const string TILE_HOVER_ENTER_ANIMATION_TRIGGER = "HoverEnter";
    private const string TILE_HOVER_EXIT_ANIMATION_TRIGGER = "HoverExit";
        
        

    [SerializeField] private Animator animator;

    [SerializeField] private Transform buildingsRoot = new RectTransform();
    
    [SerializeField] private Transform citizensRoot = new RectTransform();
    
    [SerializeField] private ShadowOverlayVisuals shadowOverlayVisuals;

    [SerializeField] private ShadowOverlayVisuals grayOutOverlayVisuals;

    [SerializeField] private TilePreviewOverlayVisuals tilePreviewOverlayVisuals;

    [SerializeField] private float bonusTickParticleSystemStartDelay = 0.15f;
    
    [SerializeField] private GameObject bonusTickParticleSystemPrefab;

    [SerializeField] private float bonusTickParticleSystemDuration = 3f;

    private List<ResourceIcon> _instantiatedResourceIcons = new List<ResourceIcon>();

    private Renderer[] _tileRenderers;

    private List<BuildingBehaviour> _attachedBuildings = new();

    private List<CitizenBehaviour> _attachedCitizens = new();

    private bool _playedAppearAnimation = false;

    private bool _hoveredOver = false;

    private void Awake()
    {
        CollectRenderers();
    }

    private void Start()
    {
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
        GlobalSettings.OnGameSpeedChanged += OnGameSpeedChanged;

        PhaseStateMachine.Instance.OnPhaseTransitionStarted -= OnPhaseTransitionStarted;
        PhaseStateMachine.Instance.OnPhaseTransitionStarted += OnPhaseTransitionStarted;

        OnGameSpeedChanged(GlobalSettings.GameSpeed);
    }

    private void OnDestroy()
    {
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;

        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseTransitionStarted -= OnPhaseTransitionStarted;
        }
    }

    private void OnPhaseTransitionStarted(GamePhases phase)
    {
        if (phase == GamePhases.BloomingHarvest)
        {
            animator.speed = GlobalSettings.GameSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
    }

    public void OnHoveredOver(bool hoverEnter)
    {
        //if !hoverEnter, the hover has exited

        if (hoverEnter != _hoveredOver)
        {
            animator.ResetTrigger(TILE_HOVER_ENTER_ANIMATION_TRIGGER);
            animator.ResetTrigger(TILE_HOVER_EXIT_ANIMATION_TRIGGER);
            if (hoverEnter)
            {
                animator.SetTrigger(TILE_HOVER_ENTER_ANIMATION_TRIGGER);
            }
            else
            {
                animator.SetTrigger(TILE_HOVER_EXIT_ANIMATION_TRIGGER);
            }
            
            _hoveredOver = hoverEnter;
        }
        
        tilePreviewOverlayVisuals.ToggleHighlight(hoverEnter);
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
        building.transform.localPosition = Vector3.zero;
        building.transform.localRotation = Quaternion.identity;
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

    public void ToggleVisuals(bool isEnabled)
    {
        foreach (Renderer renderer in _tileRenderers)
        {
            renderer.enabled = isEnabled;
        }
    }

    public void TogglePreviewTile(bool isEnabled, Action onFinished = null)
    {
        
        if (isEnabled)
        {
            ToggleVisuals(false);
            foreach (Renderer renderer in tilePreviewOverlayVisuals.gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }
            
            tilePreviewOverlayVisuals.Appear(onFinished);
        }
        else
        {
            tilePreviewOverlayVisuals.Disappear(onFinished);
        }
    }

    public void ToggleShadow(bool isEnabled)
    {
        shadowOverlayVisuals.gameObject.SetActive(isEnabled);
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
        if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingHarvest ||
            PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingResourceConversion)
        {
            animator.speed = gameSpeed;
        }
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
