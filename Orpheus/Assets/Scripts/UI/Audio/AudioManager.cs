using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private string goldValueChangedAudioEventName = "GoldValueChanged";
    
    [SerializeField] private string relicTriggeredAudioEventName = "RelicTriggered";
    
    [SerializeField] private string tileTickAudioEventName = "TileTick";
    
    [SerializeField] private string tileProcessStartAudioEventName = "TileProcessStart";
    
    [SerializeField] private string tileProcessEndAudioEventName = "TileProcessEnd";
    
    private void Start()
    {
        PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        PersistentState.Instance.OnGoldValueChanged += OnGoldValueChanged;
        
        BloomingHarvestController.Instance.OnTileProcessStart -= OnTileProcessStart;
        BloomingHarvestController.Instance.OnTileProcessStart += OnTileProcessStart;
        
        BloomingHarvestController.Instance.OnTileProcessEnd -= OnTileProcessEnd;
        BloomingHarvestController.Instance.OnTileProcessEnd += OnTileProcessEnd;
        
        HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered -= OnTileTick;
        HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered += OnTileTick;
        
        BloomingResourceConversionController.Instance.OnRelicTriggered -= OnRelicTriggered;
        BloomingResourceConversionController.Instance.OnRelicTriggered += OnRelicTriggered;
    }

    private void OnGoldValueChanged(long newGoldValue)
    {
        FireOneShotAtCameraCenter(goldValueChangedAudioEventName);
    }

    private void OnTileProcessStart(Vector2Int position, (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) resources)
    {
        FMODUnity.RuntimeManager.PlayOneShot(tileProcessStartAudioEventName, MapUtils.GetTileWorldPositionFromGridPosition(position));
    }
    
    private void OnTileProcessEnd(Vector2Int position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(tileProcessEndAudioEventName, MapUtils.GetTileWorldPositionFromGridPosition(position));
    }
    
    private void OnTileTick(Vector2Int position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(tileTickAudioEventName, MapUtils.GetTileWorldPositionFromGridPosition(position));
    }

    private void OnRelicTriggered(RelicTypes relicType)
    {
        FireOneShotAtCameraCenter(relicTriggeredAudioEventName);
    }

    private void FireOneShotAtCameraCenter(string audioEventName)
    {
        CameraUtils.GetCameraCenterPointOnPlane(Camera.main, out Vector3 cameraCenterPoint);
        
        FMODUnity.RuntimeManager.PlayOneShot(audioEventName, cameraCenterPoint);
    }
}
