using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceTextOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI foodScoreText;
    
    private void Start()
    {
        RoundState.Instance.OnFoodGoalChanged -= OnGoalUpdated;
        RoundState.Instance.OnFoodGoalChanged += OnGoalUpdated;
        
        RoundState.Instance.OnWoodValueChanged -= OnWoodUpdated;
        RoundState.Instance.OnWoodValueChanged += OnWoodUpdated;
        
        RoundState.Instance.OnStoneValueChanged -= OnStoneUpdated;
        RoundState.Instance.OnStoneValueChanged += OnStoneUpdated;
        
        RoundState.Instance.OnGoldValueChanged -= OnGoldUpdated;
        RoundState.Instance.OnGoldValueChanged += OnGoldUpdated;
        
        RoundState.Instance.OnCurrentFoodScoreChanged -= OnFoodScoreUpdated;
        RoundState.Instance.OnCurrentFoodScoreChanged += OnFoodScoreUpdated;
        
        OnGoalUpdated(RoundState.Instance.CurrentFoodGoal);
        OnWoodUpdated(RoundState.Instance.CurrentWood);
        OnStoneUpdated(RoundState.Instance.CurrentStone);
        OnGoldUpdated(RoundState.Instance.CurrentGold);
        OnFoodScoreUpdated(RoundState.Instance.CurrentFoodScore);
    }

    private void OnDestroy()
    {
        if (RoundState.IsAvailable)
        {
            RoundState.Instance.OnFoodGoalChanged -= OnGoalUpdated;
            RoundState.Instance.OnWoodValueChanged -= OnWoodUpdated;
            RoundState.Instance.OnStoneValueChanged -= OnStoneUpdated;
            RoundState.Instance.OnGoldValueChanged -= OnGoldUpdated;
            RoundState.Instance.OnCurrentFoodScoreChanged -= OnFoodScoreUpdated;
        }
    }

    private void OnGoalUpdated(long newGoal)
    {
        goalText.SetText($"Goal: {newGoal}");
    }
    
    private void OnWoodUpdated(long newWood)
    {
        woodText.SetText($"Wood: {newWood}");
    }
    
    private void OnStoneUpdated(long newStone)
    {
        stoneText.SetText($"Stone: {newStone}");
    }
    
    private void OnGoldUpdated(long newGold)
    {
        goldText.SetText($"Gold: {newGold}");
    }

    private void OnFoodScoreUpdated(long newFoodScore)
    {
        foodScoreText.SetText($"Score: {newFoodScore}");
    }
    
}
