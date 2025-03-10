using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlacingBuildingTypeButton : MonoBehaviour
{
    [SerializeField] private BuildingType buildingTypeToSwitchTo;
    
    public void OnClick()
    {
        MapInteractionController.Instance.SwitchToPlaceBuildingMode(buildingTypeToSwitchTo);
    }
}
