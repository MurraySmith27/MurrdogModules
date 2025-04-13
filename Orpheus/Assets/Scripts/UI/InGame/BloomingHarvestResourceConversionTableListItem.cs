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
    [SerializeField] private string quantityEnterTriggerName = "QuantityEnter";
    [SerializeField] private string quantityChangedTriggerName = "QuantityChanged";
    [SerializeField] private string multEnterTriggerName = "MultEnter";
    [SerializeField] private string multChangedTriggerName = "MultChanged";
    [SerializeField] private string foodScoreEnterTriggerName = "FoodScoreEnter";
    [SerializeField] private string foodScoreChangedTriggerName = "FoodScoreChanged";
    [SerializeField] private string scoredTriggerName = "Score";

    [Space(10)] 
    
    [Header("UI Elements")] 
    [SerializeField] private GameObject quantityParent;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private GameObject multParent;
    [SerializeField] private TMP_Text multText;
    [SerializeField] private GameObject foodScoreParent;
    [SerializeField] private TMP_Text foodScoreText;

    [SerializeField] private ResourceIcon resourceIcon;
    
    private bool _hasShownFoodScoreExtension = false;
    private bool _hasShownMultEnter = false;
    private bool _hasShownQuantityEnter = false;

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
    
    public void Populate(ResourceType resourceType)
    {
        resourceIcon.SetIconImage(resourceType);

        AnimationUtils.ResetAnimator(animator);
        
        animator.SetTrigger(enterTriggerName);
    }

    public void SetQuantity(long score)
    {
        quantityParent.SetActive(true);
        quantityText.SetText($"{score}");

        if (!_hasShownQuantityEnter)
        {
            _hasShownQuantityEnter = true;
            animator.SetTrigger(quantityEnterTriggerName);
        }
        else
        {
            animator.SetTrigger(quantityChangedTriggerName);
        }
    }
    
    public void SetMult(double mult)
    {
        multParent.SetActive(true);
        multText.SetText($"{Math.Round(mult, 2)}");
        
        if (!_hasShownMultEnter)
        {
            _hasShownMultEnter = true;
            animator.SetTrigger(multEnterTriggerName);
        }
        else
        {
            animator.SetTrigger(multChangedTriggerName);
        }
    }

    public void SetFoodScore(long foodScore)
    {
        foodScoreParent.SetActive(true);
        foodScoreText.SetText($"{foodScore}");
        
        if (!_hasShownFoodScoreExtension)
        {
            _hasShownFoodScoreExtension = true;
            animator.SetTrigger(foodScoreEnterTriggerName);
        }
        else
        {
            animator.SetTrigger(foodScoreChangedTriggerName);
        }
    }

    public void OnScored()
    {
        animator.SetTrigger(scoredTriggerName);
    }
   
    private void OnGameSpeedChanged(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }
}
