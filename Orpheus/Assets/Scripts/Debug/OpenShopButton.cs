using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenShopButton : MonoBehaviour
{
    [SerializeField] private Button openShopButton;

    [SerializeField] private string shopPopupName = "ShopPopup";

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
        openShopButton.interactable = (phase == GamePhases.BuddingBuilding);
    }

    public void OnOpenShopButtonClicked()
    {
        UIPopupSystem.Instance.ShowPopup(shopPopupName);
    }
}
