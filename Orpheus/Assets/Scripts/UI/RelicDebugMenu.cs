using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicDebugMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentRelicText;

    [SerializeField] private Button addRelicButton;
    [SerializeField] private Button removeRelicButton;
    
    private RelicTypes _currentRelicType;

    private void Awake()
    {
        _currentRelicType = (RelicTypes)1;
    }

    private void Start()
    {
        UpdateCurrentRelicUI();
    }
    
    public void NextRelicButtonClicked()
    {
        _currentRelicType++;

        if ((int)_currentRelicType > Enum.GetValues(typeof(RelicTypes)).Length)
        {
            _currentRelicType = (RelicTypes)1;
        }

        UpdateCurrentRelicUI();
    }

    public void PreviousRelicButtonClicked()
    {
        _currentRelicType--;

        if (_currentRelicType <= 0)
        {
            _currentRelicType = (RelicTypes)Enum.GetValues(typeof(RelicTypes)).Length;
        }
        
        UpdateCurrentRelicUI();
    }

    public void AddRelicButtonClicked()
    {
        RelicSystem.Instance.AddRelic(_currentRelicType);

        UpdateCurrentRelicUI();
    }

    public void RemoveRelicButtonClicked()
    {
        RelicSystem.Instance.RemoveRelic(_currentRelicType);

        UpdateCurrentRelicUI();
    }

    private void UpdateCurrentRelicUI()
    {
        currentRelicText.text = Enum.GetName(typeof(RelicTypes), _currentRelicType);

        if (RelicSystem.Instance.HasRelic(_currentRelicType))
        {
            addRelicButton.interactable = false;
            removeRelicButton.interactable = true;
        }
        else
        {
            addRelicButton.interactable = true;
            removeRelicButton.interactable = false;
        }
    }
}
