using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandsText : MonoBehaviour
{
    [SerializeField] private TMP_Text handsText;

    private void Start()
    {
        HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        HarvestState.Instance.OnHarvestStart += OnHarvestStart;

        CitizenController.Instance.OnHandUsed -= SetHandsText;
        CitizenController.Instance.OnHandUsed += SetHandsText;
        
        SetHandsText(new());
    }

    private void OnDestroy()
    {
        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        }

        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnHandUsed -= SetHandsText;
        }
    }

    private void OnHarvestStart()
    {
        SetHandsText(new());
    }

    private void SetHandsText(Dictionary<Guid, List<CitizenController.CitizenPlacement>> citizenPlacements)
    {
        handsText.SetText($"<bounce a=0.1 f=0.5>{HarvestState.Instance.NumRemainingHands}</bounce>");
    }
}
