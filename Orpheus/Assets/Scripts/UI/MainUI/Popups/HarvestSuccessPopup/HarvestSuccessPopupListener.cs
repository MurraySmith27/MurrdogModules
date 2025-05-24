using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestSuccessPopupListener : MonoBehaviour
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
        if (phase == GamePhases.BloomingEndStep)
        {
            // UIPopupSystem.Instance.ShowPopup("HarvestSuccessPopup");
        }
    }
}
