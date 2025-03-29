using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource tileImpulseSource;
    [SerializeField] private CinemachineImpulseSource incrementImpulseSource;
    [SerializeField] private CinemachineImpulseSource decrementImpulseSource;
    [SerializeField] private CinemachineImpulseSource genericImpulseSource;

    [SerializeField] private float forceIncreasePerResource = 0.05f;
    
    private float _currentForce = 1f;
    
    private void Start()
    {
        BloomingHarvestController.Instance.OnHarvestStart -= ResetCurrentForce;
        BloomingHarvestController.Instance.OnHarvestStart += ResetCurrentForce;
        
        HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered -= OnTileAnimationTriggered;
        HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered += OnTileAnimationTriggered;
        
        BloomingHarvestResourceDisplay.Instance.OnResourceDecrementAnimationTriggered -= OnDecrementCameraShake;
        BloomingHarvestResourceDisplay.Instance.OnResourceDecrementAnimationTriggered += OnDecrementCameraShake;
        
        BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered -= OnIncrementCameraShake;
        BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered += OnIncrementCameraShake;
    }

    private void OnDestroy()
    {
        if (HarvestAnimationController.IsAvailable)
        {
            HarvestAnimationController.Instance.OnTileHarvestAnimationTriggered -= OnTileAnimationTriggered;
        }
            
        if (BloomingHarvestResourceDisplay.IsAvailable)
        {
            BloomingHarvestResourceDisplay.Instance.OnResourceDecrementAnimationTriggered -= OnDecrementCameraShake;
            BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered -= OnIncrementCameraShake;
        }
    }

    private void ResetCurrentForce()
    {
        _currentForce = 1f;
    }

    private void OnTileAnimationTriggered(Vector2Int position)
    {
        tileImpulseSource.GenerateImpulseWithForce(_currentForce);
    }

    private void OnDecrementCameraShake(ResourceType resourceType)
    {
        decrementImpulseSource.GenerateImpulse();
    }

    private void OnIncrementCameraShake(ResourceType resourceType)
    {
        incrementImpulseSource.GenerateImpulse(_currentForce);
        _currentForce += forceIncreasePerResource;
    }
    
}
