using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HarvestSuccessPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text foodRequiredText;
    [SerializeField] private TMP_Text foodHarvestedText;
    [SerializeField] private TMP_Text bonusGoldText;
    [SerializeField] private TMP_Text interestText;

    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;

        Populate();
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(GamePhases phase)
    {
        if (phase == GamePhases.BloomingEndStep)
        {
            Populate();
        }
    }

    private void Populate()
    {
        foodRequiredText.SetText($"Required: <sprite=11>{HarvestState.Instance.CurrentFoodGoal}");
        foodHarvestedText.SetText($"Harvested: <sprite=11>{HarvestState.Instance.CurrentFoodScore}");
        
        bonusGoldText.SetText($"Reward: <sprite=0>{BloomingEndStepController.Instance.LastHarvestGoldReiceived}");
        interestText.SetText($"Interest: <sprite=0>{BloomingEndStepController.Instance.LastInterestRecieved}");
    }

    public void OnNextRoundClicked()
    {
        UIPopupSystem.Instance.HidePopup("HarvestSuccessPopup");
        PhaseStateMachine.Instance.ChangePhase(GamePhases.BuddingUpkeep);
    }
    
}
