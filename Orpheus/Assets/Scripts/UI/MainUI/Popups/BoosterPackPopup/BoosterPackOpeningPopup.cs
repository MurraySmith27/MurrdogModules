using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPackOpeningPopup : MonoBehaviour
{
    [SerializeField] private BoosterPackOption boosterPackOptionPrefab;

    private List<BoosterPackOption> _instantiatedOptions = new();

    private int _currentlySelectedOptionIndex = -1;
    
    private void OnEnable()
    {
        Reset();
        
        PopulateBoosterPackContents();
    }

    private void PopulateBoosterPackContents()
    {
        
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
