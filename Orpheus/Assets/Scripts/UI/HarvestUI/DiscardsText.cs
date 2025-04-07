using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class DiscardsText : MonoBehaviour
{
    [SerializeField] private TMP_Text discardsText;

    private void Start()
    {
        HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        HarvestState.Instance.OnHarvestStart += OnHarvestStart;

        CitizenController.Instance.OnDiscardUsed -= SetDiscardsText;
        CitizenController.Instance.OnDiscardUsed += SetDiscardsText;
        
        SetDiscardsText(new(), new(), new());
    }

    private void OnDestroy()
    {
        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        }

        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnDiscardUsed -= SetDiscardsText;
        }
    }

    private void OnHarvestStart()
    {
        SetDiscardsText(new(), new(), new());
    }

    private void SetDiscardsText(Guid cityGuid, List<CitizenController.CitizenPlacement> citizensBefore, [ItemCanBeNull] List<CitizenController.CitizenPlacement> citizensAfter)
    {
        discardsText.SetText($"<bounce a=0.1 f=0.5>{HarvestState.Instance.NumRemainingDiscards}</bounce>");
    }
}