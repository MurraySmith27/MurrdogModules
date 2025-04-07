using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalText : MonoBehaviour
{
    [SerializeField] private TMP_Text goalText;

    private void Start()
    {
        HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        HarvestState.Instance.OnHarvestStart += OnHarvestStart;

        HarvestState.Instance.OnFoodGoalChanged -= SetHarvestText;
        HarvestState.Instance.OnFoodGoalChanged += SetHarvestText;
        
        SetHarvestText(HarvestState.Instance.CurrentFoodGoal);
    }

    private void OnDestroy()
    {
        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
            HarvestState.Instance.OnCurrentFoodScoreChanged -= SetHarvestText;
        }
    }

    private void OnHarvestStart()
    {
        SetHarvestText(HarvestState.Instance.CurrentFoodGoal);
    }

    private void SetHarvestText(long goal)
    {
        goalText.SetText($"<sprite index=5><incr a=0.6 f=0.5 w=3>{goal}</incr>");
    }
}
