using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPackOpeningPopup : MonoBehaviour
{
    [SerializeField] private BoosterPackOption boosterPackOptionPrefab;

    [SerializeField] private Transform boosterPackOptionParent;

    private List<BoosterPackOption> _instantiatedOptions = new();

    private int _currentlySelectedOptionIndex = -1;
    
    private void OnEnable()
    {
        Reset();
        
        PopulateBoosterPackContents();
    }

    private void PopulateBoosterPackContents()
    {
        BoosterPackSystem.BoosterPackOfferings currentOfferings = BoosterPackSystem.Instance.GetCurrentOfferings();

        foreach (TileInformation tile in currentOfferings.tiles)
        {
            _instantiatedOptions.Add(Instantiate(boosterPackOptionPrefab, boosterPackOptionParent));

            _instantiatedOptions[^1].Populate(tile);
        }
        
        foreach (RelicTypes relic in currentOfferings.relics)
        {
            _instantiatedOptions.Add(Instantiate(boosterPackOptionPrefab, boosterPackOptionParent));

            _instantiatedOptions[^1].Populate(relic);
        }
    }

    private void Reset()
    {
        _currentlySelectedOptionIndex = -1;

        foreach (BoosterPackOption boosterPackOption in _instantiatedOptions)
        {
            Destroy(boosterPackOption.gameObject);
        }
        
        _instantiatedOptions.Clear();
    }
}
