using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BoosterPackOptionTypes
{
    Tile,
    Relic,
    Building
}

public class BoosterPackOpeningPopup : MonoBehaviour
{
    private struct BoosterPackOptionData
    {
        public BoosterPackOptionTypes Type;
        public RelicTypes Relic;
        public TileInformation Tile;
        public BuildingType Building;
    }
    
    [Header("Options")]
    [SerializeField] private BoosterPackOption boosterPackOptionPrefab;
    [SerializeField] private Transform boosterPackOptionParent;

    [Space(10)]
    
    [Header("UI")] 
    [SerializeField] private Button chooseButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text refreshCostText;

    private List<(BoosterPackOption, BoosterPackOptionData)> _instantiatedOptions = new();

    private int _currentlySelectedOptionIndex = -1;

    private int _numRefreshesUsed = 0;

    private void OnEnable()
    {
        _numRefreshesUsed = 0;
        PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        PersistentState.Instance.OnGoldValueChanged += OnGoldValueChanged;
    }

    private void OnDisable()
    {
        if (PersistentState.IsAvailable)
        {
            PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
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
            refreshCostText.SetText($"FREE");
        }
        else
        {
            refreshCostText.SetText($"<sprite index=0>{currentRefreshCost}");
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

    public void PopulateBoosterPackContents()
    {
        Reset();
        
        SetRefreshButtonState();
        
        BoosterPackSystem.BoosterPackOfferings currentOfferings = BoosterPackSystem.Instance.GetCurrentOfferings();

        int optionNum = 0;

        if (currentOfferings.tiles != null)
        {
            foreach (TileInformation tile in currentOfferings.tiles)
            {
                BoosterPackOptionData data = new();
                data.Type = BoosterPackOptionTypes.Tile;
                data.Tile = tile;
                _instantiatedOptions.Add((Instantiate(boosterPackOptionPrefab, boosterPackOptionParent), data));

                int temp = optionNum;
                _instantiatedOptions[^1].Item1.Populate(tile, () => { OnOptionSelected(temp); });
                optionNum++;
            }
        }

        if (currentOfferings.relics != null) {
            foreach (RelicTypes relic in currentOfferings.relics)
            {
                BoosterPackOptionData data = new();
                data.Type = BoosterPackOptionTypes.Relic;
                data.Relic = relic;
                _instantiatedOptions.Add((Instantiate(boosterPackOptionPrefab, boosterPackOptionParent), data));

                int temp = optionNum;
                _instantiatedOptions[^1].Item1.Populate(relic, () => {OnOptionSelected(temp);});
                optionNum++;
            }
        }

        if (currentOfferings.buildings != null)
        {
            foreach (BuildingType building in currentOfferings.buildings)
            {
                BoosterPackOptionData data = new();
                data.Type = BoosterPackOptionTypes.Building;
                data.Building = building;
                _instantiatedOptions.Add((Instantiate(boosterPackOptionPrefab, boosterPackOptionParent), data));

                int temp = optionNum;
                _instantiatedOptions[^1].Item1.Populate(building, () => { OnOptionSelected(temp); });
                optionNum++;
            }
        }
    }

    private void Reset()
    {
        _currentlySelectedOptionIndex = -1;

        foreach ((BoosterPackOption, BoosterPackOptionData) pair in _instantiatedOptions)
        {
            Destroy(pair.Item1.gameObject);
        }
        
        _instantiatedOptions.Clear();

        chooseButton.interactable = false;
    }

    public void OnOptionSelected(int optionIndex)
    {
        if (_currentlySelectedOptionIndex != -1)
        {
            _instantiatedOptions[_currentlySelectedOptionIndex].Item1.ToggleSelected(false);
        }
        
        _currentlySelectedOptionIndex = optionIndex;

        if (_currentlySelectedOptionIndex != -1)
        {
            _instantiatedOptions[optionIndex].Item1.ToggleSelected(true);

            chooseButton.interactable = true;
        }
        else
        {
            chooseButton.interactable = false;
        }
    }

    public void OnChooseButtonClicked()
    {
        UIPopupSystem.Instance.HidePopup("BoosterPackOpeningPopup");
        UIPopupSystem.Instance.HidePopup("ShopPopup");

        BoosterPackOptionData chosenOption = _instantiatedOptions[_currentlySelectedOptionIndex].Item2;

        switch (chosenOption.Type)
        {
            case BoosterPackOptionTypes.Tile:
                MapInteractionController.Instance.SwitchToPlaceTileMode(chosenOption.Tile);
                break;
            case BoosterPackOptionTypes.Relic:
                RelicSystem.Instance.AddRelic(chosenOption.Relic);
                break;
            case BoosterPackOptionTypes.Building:
                MapInteractionController.Instance.SwitchToPlaceBuildingMode(chosenOption.Building);
                break;
            default:
                break;
        }
        
        BoosterPackSystem.Instance.RemoveCurrentOfferings();
    }

    public void OnSkipButtonClicked()
    {
        UIPopupSystem.Instance.HidePopup("BoosterPackOpeningPopup");
        
        BoosterPackSystem.Instance.RemoveCurrentOfferings();
    }
    
    public void OnRefreshButtonClicked()
    {
        long refreshCost = GetCurrentRefreshCost();
        if (PersistentState.Instance.CurrentGold >= refreshCost)
        {
            PersistentState.Instance.ChangeCurrentGold(-refreshCost);
            
            BoosterPackSystem.Instance.RefreshOfferings();
            
            _numRefreshesUsed++;
            PopulateBoosterPackContents();
        }
        
    }
}
