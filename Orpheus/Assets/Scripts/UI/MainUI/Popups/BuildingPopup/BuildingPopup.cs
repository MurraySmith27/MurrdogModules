using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPopup : MonoBehaviour
{
    [SerializeField] private BuildingPopupListItem buildingPopupListItemPrefab;

    [SerializeField] private Transform buildingPopupListItemParent;

    [SerializeField] private Button refreshButton;
    
    [SerializeField] private TMP_Text refreshButtonCostText;
    
    private List<(BuildingPopupListItem, BuildingType)> _listItemInstances = new List<(BuildingPopupListItem, BuildingType)>();

    private int _currentHarvest = -1;
    private int _numRefreshesUsed = 0;
    
    private void OnEnable()
    {
        PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        PersistentState.Instance.OnGoldValueChanged += OnGoldValueChanged;
        
        Populate();
    }

    private void OnDisable()
    {
        if (PersistentState.IsAvailable)
        {
            PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        }
    }

    public void Populate()
    {
        Clear();

        if (PersistentState.Instance.HarvestNumber != _currentHarvest)
        {
            _numRefreshesUsed = 0;
            _currentHarvest = PersistentState.Instance.HarvestNumber;
        }

        SetRefreshButtonState();
        
        List<BuildingType> buildingTypes = BuildingsController.Instance.GetAvailableBuildingTypes();

        List<BuildingType> offeredBuildings = RandomChanceSystem.Instance.GetCurrentlyOfferedBuildings(buildingTypes, PersistentState.Instance.HarvestNumber, _numRefreshesUsed);

        foreach (BuildingType buildingType in offeredBuildings)
        {
            BuildingPopupListItem buildingPopupListItem = Instantiate(buildingPopupListItemPrefab, buildingPopupListItemParent);
            
            _listItemInstances.Add((buildingPopupListItem, buildingType));
            buildingPopupListItem.transform.SetSiblingIndex(0);
            
            buildingPopupListItem.Populate(buildingType);
        }
    }

    private void OnGoldValueChanged(long newValue)
    {
        SetRefreshButtonState();
    }
    
    private void SetRefreshButtonState()
    {
        long currentRefreshCost = GetCurrentRefreshCost();
        if (currentRefreshCost <= 0)
        {
            refreshButtonCostText.SetText($"FREE");
        }
        else
        {
            refreshButtonCostText.SetText($"<sprite index=0>{currentRefreshCost}");
        }
        
        refreshButton.interactable = PersistentState.Instance.CurrentGold >= currentRefreshCost;
    }

    private long GetCurrentRefreshCost()
    {
        if (_numRefreshesUsed < GameConstants.NUM_FREE_BUILDING_REFRESHES)
        {
            return 0;
        }
        else return (long)(GameConstants.INITIAL_BUILDING_REFRESH_GOLD_COST *
                           Math.Pow(GameConstants.GOLD_COST_PER_BUILDING_REFRESH_MULTIPLIER,
                               (_numRefreshesUsed - GameConstants.NUM_FREE_BUILDING_REFRESHES)));
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

    public void OnDestroyBuildingClicked()
    {
        if (BuildingsController.Instance.HasDestroyBuildingCost()) 
        {
            MapInteractionController.Instance.SwitchMapInteractionMode(MapInteractionMode.DestroyBuilding);
        }
    }

    public void OnRefreshButtonClicked()
    {
        long refreshCost = GetCurrentRefreshCost();
        if (PersistentState.Instance.CurrentGold >= refreshCost)
        {
            PersistentState.Instance.ChangeCurrentGold(-refreshCost);
            _numRefreshesUsed++;
            Populate();
        }
        
    }
}
