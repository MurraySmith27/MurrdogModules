using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingHarvestResourceConversionTable : MonoBehaviour
{
    [Header("List Items")] 
    [SerializeField] private BloomingHarvestResourceConversionTableListItem listItemPrefab;
    [SerializeField] private Transform listItemsParent;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string enterAnimatorTriggerName = "Enter";
    [SerializeField] private string exitAnimatorTriggerName = "Exit";
    [SerializeField] private string tickAnimatorTriggerName = "Tick";

    private Dictionary<ResourceType, BloomingHarvestResourceConversionTableListItem> _listItems = new();
    
    private void Start()
    {
        Debug.LogError("start!");
        BloomingResourceConversionController.Instance.OnResourceConversionStart -= OnConversionStart;
        BloomingResourceConversionController.Instance.OnResourceConversionStart += OnConversionStart;
        
        BloomingResourceConversionController.Instance.OnResourceConversionEnd -= OnConversionEnd;
        BloomingResourceConversionController.Instance.OnResourceConversionEnd += OnConversionEnd;

        BloomingResourceConversionController.Instance.OnResourceConversionResourceStart -= OnResourceConversionStart;
        BloomingResourceConversionController.Instance.OnResourceConversionResourceStart += OnResourceConversionStart;
        
        BloomingResourceConversionController.Instance.OnResourceConversionQuantityProcessed -= OnQuantityProcessed;
        BloomingResourceConversionController.Instance.OnResourceConversionQuantityProcessed += OnQuantityProcessed;
        
        BloomingResourceConversionController.Instance.OnResourceConversionMultProcessed -= OnMultProcessed;
        BloomingResourceConversionController.Instance.OnResourceConversionMultProcessed += OnMultProcessed;
        
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreProcessed -= OnFoodScoreProcessed;
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreProcessed += OnFoodScoreProcessed;

        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreAddedStart -= OnFoodScoreAddedStart;
        BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreAddedStart += OnFoodScoreAddedStart;

        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
        GlobalSettings.OnGameSpeedChanged += OnGameSpeedChanged;
        
        OnGameSpeedChanged(GlobalSettings.GameSpeed);
    }

    private void OnDestroy()
    {
        if (BloomingResourceConversionController.IsAvailable)
        {
            BloomingResourceConversionController.Instance.OnResourceConversionStart -= OnConversionStart;
            BloomingResourceConversionController.Instance.OnResourceConversionEnd -= OnConversionEnd;
            BloomingResourceConversionController.Instance.OnResourceConversionResourceStart -= OnResourceConversionStart;
            BloomingResourceConversionController.Instance.OnResourceConversionQuantityProcessed -= OnQuantityProcessed;
            BloomingResourceConversionController.Instance.OnResourceConversionMultProcessed -= OnMultProcessed;
            BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreProcessed -= OnFoodScoreProcessed;
            BloomingResourceConversionController.Instance.OnResourceConversionFoodScoreAddedStart -= OnFoodScoreAddedStart;
        }
        
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
    }

    private void OnConversionStart()
    {
        Clear();
        
        Debug.LogError("On conversion Start");
        AnimationUtils.ResetAnimator(animator);
        
        animator.SetTrigger(enterAnimatorTriggerName);
    }

    private void OnConversionEnd()
    {
        animator.SetTrigger(exitAnimatorTriggerName);
    }

    private void OnResourceConversionStart(ResourceType type)
    {
        BloomingHarvestResourceConversionTableListItem newInstance = Instantiate(listItemPrefab, listItemsParent);

        _listItems.Add(type, newInstance);

        newInstance.Populate(type);
    }

    private void OnQuantityProcessed(ResourceType type, long quantity)
    {
        _listItems[type].SetQuantity(quantity);
        
        animator.SetTrigger(tickAnimatorTriggerName);
    }
    
    private void OnMultProcessed(ResourceType type, double mult)
    {
        _listItems[type].SetMult(mult);
        
        animator.SetTrigger(tickAnimatorTriggerName);
    }
    
    private void OnFoodScoreProcessed(ResourceType type, long foodScore)
    {
        _listItems[type].SetFoodScore(foodScore);
        
        animator.SetTrigger(tickAnimatorTriggerName);
    }

    private void OnFoodScoreAddedStart(ResourceType type, long score)
    {
        _listItems[type].OnScored();
    }

    private void Clear()
    {
        foreach (BloomingHarvestResourceConversionTableListItem listItem in _listItems.Values)
        {
            Destroy(listItem.gameObject);
        }

        _listItems.Clear();
    }

    private void OnGameSpeedChanged(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }
}
