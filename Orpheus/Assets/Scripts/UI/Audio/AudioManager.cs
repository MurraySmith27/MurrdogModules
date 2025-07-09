using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private string goldValueChangedAudioEventName = "GoldValueChanged";
    
    [SerializeField] private string relicTriggeredAudioEventName = "RelicTriggered";
    
    [SerializeField] private string genericUITickAudioEventName = "UITick";
    
    [SerializeField] private string genericUISlamAudioEventName = "UISlam";
    
    [SerializeField] private string tileTickAudioEventName = "TileTick";
    
    [SerializeField] private string tileProcessStartAudioEventName = "TileProcessStart";
    
    [SerializeField] private string tileProcessEndAudioEventName = "TileProcessEnd";
    
    [SerializeField] private string resourceGainedAudioEventName = "ResourceGained";
    
    [SerializeField] private string resourceLostAudioEventName = "ResourceLost";
    
    [SerializeField] private string bonusYieldSourceTile = "BonusYieldSourceTile";
    
    [SerializeField] private string tileBonusYield = "BonusYieldApplied";
    
    [SerializeField] private string buildingConstructedAudioEventName = "BuildingConstructed";
    
    [SerializeField] private string musicAudioEventName = "BGM";
    

    [Space(10)] 
    
    [Header("Parameters")] 
    [SerializeField]
    private string isPopupShowingParameterName = "IsInCardMenu";
    private string isInHarvestParameterName = "IsInHarvest";
    private string numTicksThisHarvestParameterName = "NumTicksThisHarvest";

    [FormerlySerializedAs("BGMMuffleTransitionTime")]
    [Space(10)] 
    [Header("Animation")] 
    [SerializeField] private float FadeParameterTransitionTime = 0.75f;
    [SerializeField] private AnimationCurve FadeParameterAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private int _numTicksThisHarvest = 0;

    private bool _isBGMMuffled = false;
    
    private void Start()
    {
        PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        PersistentState.Instance.OnGoldValueChanged += OnGoldValueChanged;
        
        BloomingHarvestController.Instance.OnTileProcessStart -= OnTileProcessStart;
        BloomingHarvestController.Instance.OnTileProcessStart += OnTileProcessStart;
        
        BloomingHarvestController.Instance.OnTileProcessEnd -= OnTileProcessEnd;
        BloomingHarvestController.Instance.OnTileProcessEnd += OnTileProcessEnd;
        
        BloomingHarvestController.Instance.OnRelicTriggered -= OnRelicTriggered;
        BloomingHarvestController.Instance.OnRelicTriggered += OnRelicTriggered;
        
        HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered -= OnTileTick;
        HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered += OnTileTick;

        HarvestTileBonusYieldsAnimationController.Instance.OnTileBonusYieldApplied -= OnTileYieldBonus;
        HarvestTileBonusYieldsAnimationController.Instance.OnTileBonusYieldApplied += OnTileYieldBonus;
        
        HarvestTileBonusYieldsAnimationController.Instance.OnTileBonusYieldSourceStart -= OnTileYieldBonusSourceTile;
        HarvestTileBonusYieldsAnimationController.Instance.OnTileBonusYieldSourceStart += OnTileYieldBonusSourceTile;
        
        BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered -= OnResourceIncrement;
        BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered += OnResourceIncrement;
        
        BloomingHarvestResourceDisplay.Instance.OnResourceDecrementAnimationTriggered -= OnResourceDecrement;
        BloomingHarvestResourceDisplay.Instance.OnResourceDecrementAnimationTriggered += OnResourceDecrement;
        
        BloomingResourceConversionController.Instance.OnRelicTriggered -= OnRelicTriggered;
        BloomingResourceConversionController.Instance.OnRelicTriggered += OnRelicTriggered;
        
        BloomingResourceConversionController.Instance.OnResourceConversionQuantityProcessed -= OnResourceConversionTick;
        BloomingResourceConversionController.Instance.OnResourceConversionQuantityProcessed += OnResourceConversionTick;
        
        BloomingResourceConversionController.Instance.OnResourceConversionMultProcessed -= OnResourceConversionTick;
        BloomingResourceConversionController.Instance.OnResourceConversionMultProcessed += OnResourceConversionTick;
        
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreProcessed -= OnResourceConversionTick;
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreProcessed += OnResourceConversionTick;
        
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreAddedStart -= OnResourceConversionTick;
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreAddedStart += OnResourceConversionTick;
        
        BloomingResourceConversionController.Instance.OnResourceConversionResourceStart -= OnResourceConversionResourceStart;
        BloomingResourceConversionController.Instance.OnResourceConversionResourceStart += OnResourceConversionResourceStart;

        MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        MapSystem.Instance.OnBuildingConstructed += OnBuildingConstructed;

        UIPopupSystem.Instance.OnPopupShown -= OnPopupShown;
        UIPopupSystem.Instance.OnPopupShown += OnPopupShown;
        
        UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        UIPopupSystem.Instance.OnPopupHidden += OnPopupHidden;

        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
        
        // FireOneShotAtCameraCenter(musicAudioEventName);
    }

    private void OnDestroy()
    {
        if (PersistentState.IsAvailable)
        {
            PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
            PersistentState.Instance.OnGoldValueChanged += OnGoldValueChanged;
        }

        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnTileProcessStart -= OnTileProcessStart;
            BloomingHarvestController.Instance.OnTileProcessEnd -= OnTileProcessEnd;
        }

        if (HarvestAnimationController.IsAvailable)
        {
            HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered -= OnTileTick;
        }

        if (BloomingResourceConversionController.IsAvailable)
        {
            BloomingResourceConversionController.Instance.OnRelicTriggered -= OnRelicTriggered;
            BloomingResourceConversionController.Instance.OnResourceConversionQuantityProcessed -= OnResourceConversionTick;
            BloomingResourceConversionController.Instance.OnResourceConversionMultProcessed -= OnResourceConversionTick;
            BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreProcessed -= OnResourceConversionTick;
            BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreAddedStart -= OnResourceConversionTick;
            BloomingResourceConversionController.Instance.OnResourceConversionResourceStart -= OnResourceConversionResourceStart;
        }

        if (BloomingHarvestResourceDisplay.IsAvailable)
        {
            BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered -= OnResourceIncrement;
            BloomingHarvestResourceDisplay.Instance.OnResourceDecrementAnimationTriggered -= OnResourceDecrement;
        }

        if (UIPopupSystem.IsAvailable)
        {
            UIPopupSystem.Instance.OnPopupShown -= OnPopupShown;
            UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        }
        
        FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/master");
        playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnBuildingConstructed(int row, int col, BuildingType buildingType)
    {
        FireOneShotAtCameraCenter(buildingConstructedAudioEventName);
    }

    private void PlayGenericUITick()
    {
        FireOneShotAtCameraCenter(genericUITickAudioEventName);
    }
    
    private void PlayGenericUISlam()
    {
        FireOneShotAtCameraCenter(genericUISlamAudioEventName);
    }

    private void OnResourceConversionTick(ResourceType type, double amount)
    {
        PlayGenericUITick();
    }
    
    private void OnResourceConversionTick(ResourceType type, long amount)
    {
        PlayGenericUITick();
    }

    private void OnResourceConversionResourceStart(ResourceType type)
    {
        PlayGenericUISlam();
    }

    private void OnGoldValueChanged(long newGoldValue)
    {
        FireOneShotAtCameraCenter(goldValueChangedAudioEventName);
    }

    private void OnTileProcessStart(Vector2Int position, (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) resources)
    {
        CreateAndPlayEventAtLocation(tileProcessStartAudioEventName, MapUtils.GetTileWorldPositionFromGridPosition(position));
    }
    
    private void OnTileProcessEnd(Vector2Int position)
    {
        CreateAndPlayEventAtLocation(tileProcessEndAudioEventName, MapUtils.GetTileWorldPositionFromGridPosition(position));
    }
    
    private void OnTileTick(Vector2Int position)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(numTicksThisHarvestParameterName, _numTicksThisHarvest);

        _numTicksThisHarvest++;
        
        CreateAndPlayEventAtLocation(tileTickAudioEventName, MapUtils.GetTileWorldPositionFromGridPosition(position));
    }

    private void OnRelicTriggered(RelicTypes relicType)
    {
        FireOneShotAtCameraCenter(relicTriggeredAudioEventName);
    }
    
    private void OnRelicTriggered(Vector2Int position, RelicTypes relicType, (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) resourceDiff)
    {
        FireOneShotAtCameraCenter(relicTriggeredAudioEventName);
    }
    
    private void OnResourceIncrement(ResourceType resourceType)
    {
        FireOneShotAtCameraCenter(resourceGainedAudioEventName);
    }
    
    private void OnResourceDecrement(ResourceType resourceType)
    {
        FireOneShotAtCameraCenter(resourceLostAudioEventName);
    }

    private void OnTileYieldBonus(Vector2Int sourcePosition, Vector2Int targetPosition, int tileYield)
    {
        FireOneShotAtCameraCenter(tileBonusYield);
    }
    
    private void OnTileYieldBonusSourceTile(Vector2Int position)
    {
        FireOneShotAtCameraCenter(bonusYieldSourceTile);
    }

    private void OnPopupShown(string popupName)
    {
        if (!_isBGMMuffled)
        {
            Timing.RunCoroutine(FadeParameterTo(isPopupShowingParameterName, 1f), this.gameObject);
            _isBGMMuffled = true;
        }
    }

    private void OnPopupHidden(string popupName)
    {
        if (_isBGMMuffled && UIPopupSystem.Instance.IsPopupShowing())
        {
            Timing.RunCoroutine(FadeParameterTo(isPopupShowingParameterName,0f), this.gameObject);
            _isBGMMuffled = false;
        }
    }
    
    private void OnPhaseChanged(GamePhases gamePhase)
    {
        if (gamePhase == GamePhases.BloomingUpkeep)
        {
            Timing.RunCoroutine(FadeParameterTo(isInHarvestParameterName, 1f), this.gameObject);
        }
        else if (gamePhase == GamePhases.BuddingUpkeep)
        {
            Timing.RunCoroutine(FadeParameterTo(isInHarvestParameterName, 0f), this.gameObject);
        }
    }

    private IEnumerator<float> FadeParameterTo(string parameterName, float finalValue)
    {
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName(parameterName, out float initialValue);

        for (float t = 0; t < FadeParameterTransitionTime; t += Time.deltaTime)
        {
            float progress = FadeParameterAnimationCurve.Evaluate(t / FadeParameterTransitionTime);
            
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(parameterName, Mathf.Lerp(initialValue, finalValue, progress));
            yield return Timing.WaitForOneFrame;
        }
        
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(parameterName, finalValue);
    }

    private void FireOneShotAtCameraCenter(string audioEventName)
    {
        CameraUtils.GetCameraCenterPointOnPlane(Camera.main, out Vector3 cameraCenterPoint);
        
        CreateAndPlayEventAtLocation(audioEventName, cameraCenterPoint);
    }

    private void CreateAndPlayEventAtLocation(string eventName, Vector3 location)
    {
        var eventInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/{eventName}");
        eventInstance.start();
        
        // FMODUnity.RuntimeManager.(eventInstance, location);
        eventInstance.release();
    }

}
