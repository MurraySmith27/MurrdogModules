using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundRewardsOptionsPopupListener : MonoBehaviour
{
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
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
        if (phase == GamePhases.BuddingUpkeep)
        {
            if (PersistentState.Instance.HarvestNumber > 0)
            {
                BoosterPackSystem.Instance.OpenBoosterPack(BoosterPackTypes.ROUND_REWARDS_BUILDING_BOOSTER);
                // UIPopupSystem.Instance.ShowPopup("RoundRewardsOptionsPopup");
            }
        }
    }
}
