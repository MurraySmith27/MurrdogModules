using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenShopButton : MonoBehaviour
{
    [SerializeField] private Button openShopButton;

    [SerializeField] private string shopPopupName = "ShopPopup";
    
    [SerializeField] private Animator animator;
    
    [SerializeField] private string shopAnimatorEnterTriggerName = "Enter";
    
    [SerializeField] private string shopAnimatorExitTriggerName = "Exit";

    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;

        UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        UIPopupSystem.Instance.OnPopupHidden += OnPopupHidden;
        
        animator.speed = 1.3f;
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }

        if (UIPopupSystem.IsAvailable)
        {
            UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        }
    }

    private void OnEnable()
    {
        AnimatorEnter();
    }

    private void OnPhaseChanged(GamePhases phase)
    {
        openShopButton.interactable = (phase == GamePhases.BuddingBuilding);
    }

    public void OnOpenShopButtonClicked()
    {
        if (UIPopupSystem.Instance.IsPopupShowing("BuildingPopup"))
        {
            UIPopupSystem.Instance.HidePopup("BuildingPopup");
        }
        
        if (UIPopupSystem.Instance.IsPopupShowing(shopPopupName))
        {
            UIPopupSystem.Instance.HidePopup(shopPopupName);
        }
        else
        {
            UIPopupSystem.Instance.ShowPopup(shopPopupName);
            AnimatorExit();
        }
    }

    private void OnPopupHidden(string popupName)
    {
        if (popupName == "ShopPopup")
        {
            AnimatorEnter();
        }
    }

    private void AnimatorEnter()
    {
        AnimationUtils.ResetAnimator(animator);
        animator.SetTrigger(shopAnimatorEnterTriggerName);
    }

    private void AnimatorExit()
    {
        animator.SetTrigger(shopAnimatorExitTriggerName);
    }
}
