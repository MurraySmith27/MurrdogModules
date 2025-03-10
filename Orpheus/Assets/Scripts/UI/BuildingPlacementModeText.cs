using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentPlacingModeText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingPlacementModeText;
    
    void Start()
    {
        MapInteractionController.Instance.OnPlacingBuildingTypeChanged -= OnPlacingBuildingTypeChanged;
        MapInteractionController.Instance.OnPlacingBuildingTypeChanged += OnPlacingBuildingTypeChanged;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnPlacingBuildingTypeChanged -= OnPlacingBuildingTypeChanged;
        }
    }

    private void OnPlacingBuildingTypeChanged(BuildingType buildingType)
    {
        buildingPlacementModeText.text = Enum.GetName(typeof(BuildingType), buildingType);
    }
}
