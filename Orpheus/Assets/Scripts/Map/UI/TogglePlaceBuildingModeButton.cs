using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TogglePlaceBuildingModeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;

    [SerializeField] private BuildingType buildingType;
    
    private void Start()
    {
        buttonText.SetText("Tile Select Mode");

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
        SetButtonText();
    }

    private void SetButtonText()
    {
        switch (MapInteractionController.Instance.CurrentMode)
        {
            case (MapInteractionMode.Default):
                buttonText.SetText($"Place {Enum.GetName(typeof(BuildingType), buildingType)}");
                break;
            case (MapInteractionMode.PlaceBuilding):
                buttonText.SetText("Tile Select Mode");
                break;
        }
    } 
    
    private void SwitchToPlaceBuildingMode()
    {
        MapInteractionController.Instance.SwitchToPlaceBuildingMode(buildingType);
        SetButtonText();
    }

    private void SwitchToDefaultMode()
    {
        MapInteractionController.Instance.SwitchMapInteractionMode(MapInteractionMode.Default);
        SetButtonText();
    }

    public void OnClick()
    {
        if (MapInteractionController.Instance.CurrentMode == MapInteractionMode.Default)
        {
            SwitchToPlaceBuildingMode();
        }
        else
        {
            SwitchToDefaultMode();
        }
    }
}
