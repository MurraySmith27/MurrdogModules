using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BloomingHarvestResourceConversionTableListItem : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string enterTriggerName = "Enter";
    [SerializeField] private string quantityChangedTriggerName = "QuantityChanged";
    [SerializeField] private string multChangedTriggerName = "MultChanged";
    [SerializeField] private string foodScoreEnterTriggerName = "FoodScoreEnter";
    [SerializeField] private string foodScoreChangedTriggerName = "FoodScoreChanged";
    
    [Space(10)]
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text multText;
    [SerializeField] private TMP_Text foodScoreText;

    [SerializeField] private ResourceIcon resourceIcon;

    private bool _hasShownFoodScoreExtension = false;

    public void Populate(ResourceType resourceType)
    {
        resourceIcon.SetIconImage(resourceType);

        AnimationUtils.ResetAnimator(animator);
        
        animator.SetTrigger(enterTriggerName);
    }

    public void SetQuantity(long score)
    {
        quantityText.gameObject.SetActive(true);
        quantityText.SetText($"{score}");
        
        animator.SetTrigger(quantityChangedTriggerName);
    }
    
    public void SetMult(double mult)
    {
        multText.gameObject.SetActive(true);
        multText.SetText($"{Math.Round(mult, 2)}");
        animator.SetTrigger(multChangedTriggerName);
    }

    public void SetFoodScore(long foodScore)
    {
        foodScoreText.gameObject.SetActive(true);
        foodScoreText.SetText($"{foodScore}");
        
        if (!_hasShownFoodScoreExtension)
        {
            _hasShownFoodScoreExtension = true;
            animator.SetTrigger(foodScoreEnterTriggerName);
        }
        
        animator.SetTrigger(foodScoreChangedTriggerName);
    }
   
}
