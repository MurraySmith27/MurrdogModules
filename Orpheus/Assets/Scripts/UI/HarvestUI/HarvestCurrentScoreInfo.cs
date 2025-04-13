using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestCurrentScoreInfo : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string tickTriggerName = "Tick";
    private void Start()
    {
        HarvestState.Instance.OnCurrentFoodScoreChanged -= OnFoodScoreChanged;
        HarvestState.Instance.OnCurrentFoodScoreChanged += OnFoodScoreChanged;
    }

    private void OnDestroy()
    {
        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnCurrentFoodScoreChanged -= OnFoodScoreChanged;
        }
    }

    private void OnFoodScoreChanged(long foodScore)
    {
        animator.SetTrigger(tickTriggerName);
    }
}
