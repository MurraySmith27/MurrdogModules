using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BloomingHarvestResourceListItem : MonoBehaviour
{
    [SerializeField] private ResourceIcon resourceIcon;

    [SerializeField] private TextMeshProUGUI quantityText;

    [Space(10)]
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string activateAnimatorTriggerName = "Activate";
    [SerializeField] private string incrementAnimatorTriggerName = "Increment";
    [SerializeField] private string decrementAnimatorTriggerName = "Decrement";

    [SerializeField] private float incrementTextWaitTime = 0.5f;
    [SerializeField] private float decrementTextWaitTime = 0.05f;

    [SerializeField] private TextMeshProUGUI incrementText;
    [SerializeField] private TextMeshProUGUI decrementText;

    private long _currentQuantity;
    
    public void Populate(ResourceType type, long quantity)
    {
        quantityText.SetText($"{quantity}");
        
        resourceIcon.SetIconImage(type);

        _currentQuantity = quantity;
        
        animator.SetTrigger(activateAnimatorTriggerName);
    }

    public void ModifyQuantity(long quantityDifference)
    {
        if (quantityDifference > 0)
        {
            incrementText.SetText($"{quantityDifference}");
            
            Timing.RunCoroutineSingleton(SetMainTextAfterSeconds(incrementTextWaitTime), this.gameObject,
                SingletonBehavior.Overwrite);
            
            Debug.LogError("LIST ITEM INCREMENT TRIGGER");
            animator.SetTrigger(incrementAnimatorTriggerName);
        }
        else if (quantityDifference < 0)
        {
            decrementText.SetText($"{-quantityDifference}");

            Timing.RunCoroutineSingleton(SetMainTextAfterSeconds(decrementTextWaitTime), this.gameObject,
                SingletonBehavior.Overwrite);
            animator.SetTrigger(decrementAnimatorTriggerName);
        }

        _currentQuantity += quantityDifference;
    }

    private IEnumerator<float> SetMainTextAfterSeconds(float seconds)
    {
        yield return OrpheusTiming.WaitForSecondsGameTime(seconds);
        
        quantityText.SetText($"{_currentQuantity}");
    }

    private void Start()
    {
        ResetAnimator();
        
        animator.SetTrigger(activateAnimatorTriggerName);

        GlobalSettings.OnGameSpeedChanged -= SetGameSpeed;
        GlobalSettings.OnGameSpeedChanged += SetGameSpeed;
    }

    private void OnDestroy()
    {
        GlobalSettings.OnGameSpeedChanged -= SetGameSpeed;
    }

    private void ResetAnimator()
    {
        animator.Play("None");

        foreach (var param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
    }

    private void SetGameSpeed(float gameSpeed)
    {
        animator.speed = gameSpeed;
        Debug.LogError($"setting animator speed to {gameSpeed}");
    }
}
