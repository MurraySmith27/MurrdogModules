using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPopup : MonoBehaviour
{
    [SerializeField] private BuildingPopupListItem buildingPopupListItemPrefab;

    [SerializeField] private Transform buildingPopupListItemParent;
    
    private List<(BuildingPopupListItem, BuildingType)> _listItemInstances = new List<(BuildingPopupListItem, BuildingType)>();
    
    private void OnEnable()
    {
        Populate();
    }
    
    public void Populate()
    {
        Clear();
        
        List<BuildingType> buildingTypes = BuildingsController.Instance.GetAvailableBuildingTypes();

        List<BuildingType> offeredBuildings = RandomChanceSystem.Instance.GetCurrentlyOfferedBuildings(buildingTypes, PersistentState.Instance.HarvestNumber);

        foreach (BuildingType buildingType in offeredBuildings)
        {
            BuildingPopupListItem buildingPopupListItem = Instantiate(buildingPopupListItemPrefab, buildingPopupListItemParent);
            
            _listItemInstances.Add((buildingPopupListItem, buildingType));
            
            buildingPopupListItem.Populate(buildingType);
        }
    }

    private void Clear()
    {
        foreach ((BuildingPopupListItem, BuildingType) pair in _listItemInstances)
        {
            Destroy(pair.Item1.gameObject);
        }
        
        _listItemInstances.Clear();        
    }

    public void OnCloseButtonClicked()
    {
        UIPopupSystem.Instance.HidePopup("BuildingPopup");
    }
}
