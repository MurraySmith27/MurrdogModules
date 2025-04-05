using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestRoundControlsButtons : MonoBehaviour
{
    [SerializeField] private Button playHandButton;
    [SerializeField] private Button discardButton;
    
    private void Start()
    {
        CitizenController.Instance.OnHandUsed -= OnHandUsed;
        CitizenController.Instance.OnHandUsed += OnHandUsed;

        CitizenController.Instance.OnDiscardUsed -= OnDiscardUsed;
        CitizenController.Instance.OnDiscardUsed += OnDiscardUsed;

        HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        HarvestState.Instance.OnHarvestStart += OnHarvestStart;
    }

    private void OnDestroy()
    {
        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnHandUsed -= OnHandUsed;
            CitizenController.Instance.OnDiscardUsed -= OnDiscardUsed;
        }

        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        }
    }

    private void OnHandUsed(Dictionary<Guid, List<CitizenController.CitizenPlacement>> citizenPositions)
    {
        playHandButton.interactable = HarvestState.Instance.NumRemainingHands > 0;
    }

    private void OnDiscardUsed(
        Guid cityGuid, 
        List<CitizenController.CitizenPlacement> citizenPositionsBefore, 
        List<CitizenController.CitizenPlacement> citizenPositionsAfter
        )
    {
        discardButton.interactable = HarvestState.Instance.NumRemainingDiscards > 0;
    }

    private void OnHarvestStart()
    {
        playHandButton.interactable = true;
    }
    
    public void OnPlayHandClick()
    {
        if (HarvestState.Instance.NumRemainingHands > 0)
        {
            CitizenController.Instance.UseHand();
        }
    }
    
    public void OnDiscardClick()
    {
        if (HarvestState.Instance.NumRemainingDiscards > 0)
        {
            CitizenController.Instance.UseDiscard(MapSystem.Instance.GetAllCityGuids()[0]);
        }
    }
}
