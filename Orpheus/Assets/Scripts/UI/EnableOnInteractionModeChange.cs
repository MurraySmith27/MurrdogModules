using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnableOnInteractionModeChange : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private List<MapInteractionMode> interactionModesToEnableFor = new();
    [SerializeField] private List<BuildingType> buildingTypesToEnableForInPlaceBuilding = new();
    
    private void Start()
    {
        MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        MapInteractionController.Instance.OnMapInteractionModeChanged += OnMapInteractionModeChanged;

        if (buildingTypesToEnableForInPlaceBuilding.Count > 0)
        {
            MapInteractionController.Instance.OnPlacingBuildingTypeChanged -= OnPlacingBuildingTypeChanged;
            MapInteractionController.Instance.OnPlacingBuildingTypeChanged += OnPlacingBuildingTypeChanged;
        }
        
        OnMapInteractionModeChanged(MapInteractionController.Instance.CurrentMode);
    }
    
    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
            MapInteractionController.Instance.OnPlacingBuildingTypeChanged -= OnPlacingBuildingTypeChanged;
        }
    }

    private void OnMapInteractionModeChanged(MapInteractionMode newInteractionMode)
    {
        if (interactionModesToEnableFor.Contains(newInteractionMode))
        {

            if (newInteractionMode == MapInteractionMode.PlaceBuilding)
            {
                BuildingType currentBuildingType = MapInteractionController.Instance.CurrentlyPlacingBuildingType;

                if (buildingTypesToEnableForInPlaceBuilding.Count == 0 || buildingTypesToEnableForInPlaceBuilding.Contains(currentBuildingType))
                {
                    objectToEnable.SetActive(true);
                }
                else
                {
                    objectToEnable.SetActive(false);
                }
            }
            else objectToEnable.SetActive(true);
        }
        else
        {
            objectToEnable.SetActive(false);
        }
    }

    private void OnPlacingBuildingTypeChanged(BuildingType buildingType)
    {
        if (MapInteractionController.Instance.CurrentMode == MapInteractionMode.PlaceBuilding)
        {
            if (buildingTypesToEnableForInPlaceBuilding.Contains(buildingType))
            {
                objectToEnable.SetActive(true);
            }
            else
            {
                objectToEnable.SetActive(false);
            }
        }
    }
}
