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
        PhaseStateMachine.Instance.OnPhaseEnterComplete -= OnPhaseEnterComplete;
        PhaseStateMachine.Instance.OnPhaseEnterComplete += OnPhaseEnterComplete;
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseEnterComplete -= OnPhaseEnterComplete;
        }
    }
    
    public void OnClick()
    {
        goNextPhaseButton.interactable = false;
        PhaseStateMachine.Instance.ChangePhase((GamePhases)(((int)PhaseStateMachine.Instance.CurrentPhase + 1) % Enum.GetNames(typeof(GamePhases)).Length));
    }

    private void OnPhaseEnterComplete(GamePhases phase)
    {
        goNextPhaseButton.interactable = true;
    }
}
