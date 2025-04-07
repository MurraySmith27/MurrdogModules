using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoNextPhaseButton : MonoBehaviour
{
    [SerializeField] private Button goNextPhaseButton;
    
    private void Awake()
    {
        if (goNextPhaseButton == null)
        {
            goNextPhaseButton = GetComponentInChildren<Button>();
        }
    }
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
    
    public void OnClick()
    {
        goNextPhaseButton.interactable = false;
        RoundController.Instance.GoToNextPhase();
    }

    private void OnPhaseChanged(GamePhases phase)
    {
        goNextPhaseButton.interactable = RoundController.Instance.IsInInteractableRound();
    }
}
