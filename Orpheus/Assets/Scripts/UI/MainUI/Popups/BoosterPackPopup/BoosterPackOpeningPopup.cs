using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BoosterPackOptionTypes
{
    Tile,
    Relic,
}

public class BoosterPackOpeningPopup : MonoBehaviour
{
    private struct BoosterPackOptionData
    {
        public BoosterPackOptionTypes Type;
        public RelicTypes Relic;
        public TileInformation Tile;
    }
    
    [Header("Options")]
    [SerializeField] private BoosterPackOption boosterPackOptionPrefab;
    [SerializeField] private Transform boosterPackOptionParent;

    [Space(10)]
    
    [Header("UI")] 
    [SerializeField] private Button chooseButton;

    private List<(BoosterPackOption, BoosterPackOptionData)> _instantiatedOptions = new();

    private int _currentlySelectedOptionIndex = -1;

    public void PopulateBoosterPackContents()
    {
        Reset();
        
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
}
