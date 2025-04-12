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
        }
    }

    private void OnConversionStart()
    {
        Clear();
        
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

    private void Clear()
    {
        foreach (BloomingHarvestResourceConversionTableListItem listItem in _listItems.Values)
        {
            Destroy(listItem.gameObject);
        }

        _listItems.Clear();
    }
}
