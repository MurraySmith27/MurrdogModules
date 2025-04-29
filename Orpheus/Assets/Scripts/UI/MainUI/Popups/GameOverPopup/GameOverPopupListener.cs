using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopupListener : MonoBehaviour
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
        if (phase == GamePhases.GameOver)
        {
            UIPopupSystem.Instance.ShowPopup("GameOverPopup");
        }
    }
}
