using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoNextPhaseButton : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button goNextPhaseButton;

    [SerializeField] private string animatorEnterTriggerName = "Enter";
    [SerializeField] private string animatorExitTriggerName = "Exit";
    
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
        RoundController.Instance.GoToNextPhase();
    }

    private bool _isActive = false;
    
    private void OnPhaseChanged(GamePhases phase)
    {
        if (phase == GamePhases.BuddingBuilding)
        {
            AnimationUtils.ResetAnimator(animator);
            animator.SetTrigger(animatorEnterTriggerName);
            _isActive = true;
        }
        else if (_isActive)
        {
            animator.SetTrigger(animatorExitTriggerName);
            _isActive = false;
        }
        
    }
}
