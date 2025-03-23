using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Unity.VisualScripting;
using UnityEngine;

public class BloomingHarvestResourceDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform rootTransform;
    [SerializeField] private RectTransform listParentTransform;
    [SerializeField] private BloomingHarvestResourceListItem bloomingHarvestResourceListItemPrefab;
    
    [Space(10)]
    
    [Header("Animation Params")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorActivateTriggerName = "Activate";
    [SerializeField] private string animatorDeactivateTriggerName = "Deactivate";
    [SerializeField] private string animatorMoveTriggerName = "Move";
    [SerializeField] private string animatorIncrementResourceTriggerName = "IncrementResource";
    [SerializeField] private string animatorDecrementResourceTriggerName = "DecrementResource";
    [SerializeField] private AnimationCurve movementAnimCurve;
    [SerializeField] private float movementTimeSeconds = 0.2f;
    [SerializeField] private float baseWaitTimePerResourceChange = 0.1f;
    [SerializeField] private float waitBeforeIncrementAnim = 0.2f;
    [SerializeField] private float waitBeforeDecrementAnim = 0.05f;
    [SerializeField] private float waitBeforeCreateListItemAnim = 0.5f;
    [SerializeField] private float verticalOffset = 1f;
    
    private Dictionary<ResourceType, (int, BloomingHarvestResourceListItem)> _instantiatedListItemsPerResourceType = new();

    private Camera _mainCamera;

    private void Start()
    {
        BloomingHarvestController.Instance.OnCityHarvestStart -= OnCityHarvestStart;
        BloomingHarvestController.Instance.OnCityHarvestStart += OnCityHarvestStart;
        
        BloomingHarvestController.Instance.OnCityHarvestEnd -= OnCityHarvestEnd;
        BloomingHarvestController.Instance.OnCityHarvestEnd += OnCityHarvestEnd;
        
        BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvested;
        BloomingHarvestController.Instance.OnTileHarvestStart += OnTileHarvested;

        GlobalSettings.OnGameSpeedChanged -= SetGameSpeed;
        GlobalSettings.OnGameSpeedChanged += SetGameSpeed;

        SetGameSpeed(GlobalSettings.GameSpeed);
        
        _mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnCityHarvestStart -= OnCityHarvestStart;
            BloomingHarvestController.Instance.OnCityHarvestEnd -= OnCityHarvestEnd;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvested;
        }
        
        GlobalSettings.OnGameSpeedChanged -= SetGameSpeed;
    }

    private void OnCityHarvestStart(Guid cityGuid)
    {
        Reset();
        
        Vector2Int cityCenter = MapSystem.Instance.GetCityCenterPosition(cityGuid);

        AnimatePositionToTile(cityCenter);
        
        transform.localRotation = Quaternion.Euler(-Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up).eulerAngles.x + 90, 0, 0);

        ResetAnimatorState();
        
        animator.SetTrigger(animatorActivateTriggerName);
    }

    private void ResetAnimatorState()
    {
        animator.Play("None");
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
    }

    private void OnCityHarvestEnd(Guid cityGuid)
    {
        animator.SetTrigger(animatorDeactivateTriggerName);
    }

    private void OnTileHarvested(Vector2Int position, Dictionary<ResourceType, int> resourcesHarvested)
    {
        Timing.RunCoroutineSingleton(HarvestTileCoroutine(position, resourcesHarvested), this.gameObject,
            SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> HarvestTileCoroutine(Vector2Int position, Dictionary<ResourceType, int> resourcesHarvested)
    {
        AnimatePositionToTile(position);
        
        foreach (ResourceType resourceType in resourcesHarvested.Keys)
        {
            if (resourcesHarvested[resourceType] == 0) continue;
            
            float timeToWait = baseWaitTimePerResourceChange;
            
            int existingQuantity = 0;
            BloomingHarvestResourceListItem newListItem;
            if (_instantiatedListItemsPerResourceType.ContainsKey(resourceType))
            {
                existingQuantity = _instantiatedListItemsPerResourceType[resourceType].Item1;
                newListItem = _instantiatedListItemsPerResourceType[resourceType].Item2;
                newListItem.ModifyQuantity(resourcesHarvested[resourceType]);
                
                if (resourcesHarvested[resourceType] > 0)
                {
                    OrpheusTiming.InvokeCallbackAfterSecondsGameTime(waitBeforeIncrementAnim, () =>
                    {
                        animator.SetTrigger(animatorIncrementResourceTriggerName);
                    });

                    timeToWait += waitBeforeIncrementAnim;
                }
                else if (resourcesHarvested[resourceType] < 0)
                {
                    OrpheusTiming.InvokeCallbackAfterSecondsGameTime(waitBeforeDecrementAnim, () =>
                    {
                        animator.SetTrigger(animatorDecrementResourceTriggerName);
                    });
                    
                    timeToWait += waitBeforeDecrementAnim;
                }
            }
            else
            {
                newListItem = Instantiate(bloomingHarvestResourceListItemPrefab, listParentTransform);
                newListItem.Populate(resourceType, resourcesHarvested[resourceType]);
                
                OrpheusTiming.InvokeCallbackAfterSecondsGameTime(waitBeforeCreateListItemAnim, () =>
                {
                    animator.SetTrigger(animatorIncrementResourceTriggerName);
                });
                
                timeToWait += waitBeforeCreateListItemAnim;
            }
            
            int newQuantity = existingQuantity + resourcesHarvested[resourceType];

            _instantiatedListItemsPerResourceType[resourceType] = (newQuantity, newListItem);

            yield return OrpheusTiming.WaitForSecondsGameTime(timeToWait);
        }
    }



    private void AnimatePositionToTile(Vector2Int tilePosition)
    {
        MEC.Timing.RunCoroutine(AnimatePositionToTileCoroutine(tilePosition), this.gameObject);
    }

    private IEnumerator<float> AnimatePositionToTileCoroutine(Vector2Int tilePosition)
    {
        Vector3 originalPosition = rootTransform.localPosition;

        Vector3 worldPosition = MapUtils.GetTileWorldPositionFromGridPosition(tilePosition) + verticalOffset * _mainCamera.transform.up;
        
        Vector3 destinationPosition = rootTransform.parent.InverseTransformPoint(worldPosition);

        float movementTime = OrpheusTiming.ConvertToGameTimeSeconds(movementTimeSeconds);
        
        for (float t = 0f; t < movementTime; t += Time.deltaTime)
        {
            float normalizedT = t / movementTime;
        
            rootTransform.localPosition = Vector3.Lerp(originalPosition, destinationPosition,
                movementAnimCurve.Evaluate(normalizedT));
            
            yield return 0;
        }

        rootTransform.localPosition = destinationPosition;
    }
    
    private void Reset()
    {
        foreach (ResourceType resourceType in _instantiatedListItemsPerResourceType.Keys)
        {
            Destroy(_instantiatedListItemsPerResourceType[resourceType].Item2.gameObject);
        }
        
        _instantiatedListItemsPerResourceType.Clear();
    }
    
    private void Update()
    {
        rootTransform.localRotation = Quaternion.Euler(Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up).eulerAngles.x - 90, 0, 0);
    }

    private void SetGameSpeed(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }
}
