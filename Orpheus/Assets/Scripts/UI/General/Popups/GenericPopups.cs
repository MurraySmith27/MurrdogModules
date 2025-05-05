using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenericPopups
{
    public static void ShowSingleRedButton(string buttonText, Action onClickCallback)
    {
        UIPopupComponent popup = UIPopupSystem.Instance.ShowPopup("GenericButtonsPopup", PanelShowBehaviour.HIDE_PREVIOUS);
        if (popup != null)
        {
            GenericButtonsPopup genericButtonsPopup = popup.GetComponent<GenericButtonsPopup>();
            
            genericButtonsPopup.ShowRedButton(buttonText, onClickCallback);
        }
    }
    
    public static void ShowSingleButton(string buttonText, Action onClickCallback)
    {
        UIPopupComponent popup = UIPopupSystem.Instance.ShowPopup("GenericButtonsPopup", PanelShowBehaviour.HIDE_PREVIOUS);
        if (popup != null)
        {
            GenericButtonsPopup genericButtonsPopup = popup.GetComponent<GenericButtonsPopup>();
            
            genericButtonsPopup.ShowBlueButton(buttonText, onClickCallback);
        }
    }
    
    public static void ShowDoubleButtons(string buttonText1, string buttonText2, Action onClickCallback1, Action onClickCallback2)
    {
        UIPopupComponent popup = UIPopupSystem.Instance.ShowPopup("GenericButtonsPopup", PanelShowBehaviour.HIDE_PREVIOUS);
        if (popup != null)
        {
            GenericButtonsPopup genericButtonsPopup = popup.GetComponent<GenericButtonsPopup>();
            
            genericButtonsPopup.ShowDoubleButtons(buttonText1, buttonText2, onClickCallback1, onClickCallback2);
        }
    }
    
}
