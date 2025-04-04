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
        HarvestState.Instance.OnFoodGoalChanged -= OnGoalUpdated;
        HarvestState.Instance.OnFoodGoalChanged += OnGoalUpdated;
        
        PersistentState.Instance.OnWoodValueChanged -= OnWoodUpdated;
        PersistentState.Instance.OnWoodValueChanged += OnWoodUpdated;
        
        PersistentState.Instance.OnStoneValueChanged -= OnStoneUpdated;
        PersistentState.Instance.OnStoneValueChanged += OnStoneUpdated;
        
        PersistentState.Instance.OnGoldValueChanged -= OnGoldUpdated;
        PersistentState.Instance.OnGoldValueChanged += OnGoldUpdated;
        
        HarvestState.Instance.OnCurrentFoodScoreChanged -= OnFoodScoreUpdated;
        HarvestState.Instance.OnCurrentFoodScoreChanged += OnFoodScoreUpdated;
        
        OnGoalUpdated(HarvestState.Instance.CurrentFoodGoal);
        OnWoodUpdated(PersistentState.Instance.CurrentWood);
        OnStoneUpdated(PersistentState.Instance.CurrentStone);
        OnGoldUpdated(PersistentState.Instance.CurrentGold);
        OnFoodScoreUpdated(HarvestState.Instance.CurrentFoodScore);
    }

    private void OnDestroy()
    {
        if (PersistentState.IsAvailable)
        {
            HarvestState.Instance.OnFoodGoalChanged -= OnGoalUpdated;
            PersistentState.Instance.OnWoodValueChanged -= OnWoodUpdated;
            PersistentState.Instance.OnStoneValueChanged -= OnStoneUpdated;
            PersistentState.Instance.OnGoldValueChanged -= OnGoldUpdated;
            HarvestState.Instance.OnCurrentFoodScoreChanged -= OnFoodScoreUpdated;
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
