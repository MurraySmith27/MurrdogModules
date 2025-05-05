using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class GenericButtonsPopup : MonoBehaviour
{
    [SerializeField] private Button singleRedButton;
    [SerializeField] private Button singleBlueButton;
    
    [SerializeField] private GameObject doubleButtonsRoot;
    [SerializeField] private Button doubleButtonLeft;
    [SerializeField] private Button doubleButtonRight;

    public void ShowRedButton(string buttonText, Action onClick)
    {
        Reset();
        
        SetUpButton(singleRedButton, buttonText, onClick);
    }

    public void ShowBlueButton(string buttonText, Action onClick)
    {
        Reset();
        
        SetUpButton(singleBlueButton, buttonText, onClick);
    }

    public void ShowDoubleButtons(string buttonText1, string buttonText2, Action onClick1, Action onClick2)
    {
        Reset();
        
        SetUpButton(doubleButtonLeft, buttonText1, onClick1);
        
        SetUpButton(doubleButtonRight, buttonText2, onClick2);
    }

    private void SetUpButton(Button button, string buttonText, Action onClick)
    {
        button.gameObject.SetActive(true);
        
        button.GetComponentInChildren<TMP_Text>().SetText(buttonText);

        UnityAction listener = new(onClick);
        
        button.onClick.AddListener(listener);
    }

    private void Reset()
    {
        singleRedButton.gameObject.SetActive(false);
        singleRedButton.onClick.RemoveAllListeners();
        singleBlueButton.gameObject.SetActive(false);
        singleBlueButton.onClick.RemoveAllListeners();
        doubleButtonsRoot.SetActive(false);
        doubleButtonLeft.onClick.RemoveAllListeners();
        doubleButtonRight.onClick.RemoveAllListeners();
    }
}
