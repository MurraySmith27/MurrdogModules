using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIPopupSystemStaticAccessor
{
    public static void ShowPopup(string popupId, PanelShowBehaviour showBehaviour = PanelShowBehaviour.KEEP_PREVIOUS)
    {
        if (UIPopupSystem.Instance != null)
        {
            UIPopupSystem.Instance.ShowPopup(popupId, showBehaviour);
        }
        else
        {
            Debug.LogError("Error trying to call ShowPopup, UIPopupSystem is not initialized!");
        }
    }

    public static void HidePopup(string popupId)
    {
        if (UIPopupSystem.Instance != null)
        {
            UIPopupSystem.Instance.HidePopup(popupId);
        }
        else
        {
            Debug.LogError("Error trying to call ShowPopup, UIPopupSystem is not initialized!");
        }
    }
}
