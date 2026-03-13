#if USING_NEW_INPUT_SYSTEM
using System;
using UnityEngine;

public static class UIYesNoPopupStaticAccessor
{
    public static void RequestYesNoPopup(UIYesNoPromptType promptType, string promptText, Action onYes,
        Action onNo, PanelShowBehaviour showBehaviour = PanelShowBehaviour.HIDE_PREVIOUS)
    {
        if (UIYesNoPopupSystem.Instance != null)
        {
            UIYesNoPopupSystem.Instance.RequestYesNoPopup(promptType, promptText, onYes, onNo, showBehaviour);
        }
        else
        {
            Debug.LogError("Error trying to call RequestYesNoPopup, UIYesNoPopupSystem is not initialized!");
        }
    }

    public static void DismissYesNoPopup()
    {
        if (UIYesNoPopupSystem.Instance != null)
        {
            UIYesNoPopupSystem.Instance.DismissYesNoPopup();
        }
        else
        {
            Debug.LogError("Error trying to call DismissYesNoPopup, UIYesNoPopupSystem is not initialized!");
        }
    }

    public static bool IsYesNoPopupShowing()
    {
        if (UIYesNoPopupSystem.Instance != null)
        {
            return UIYesNoPopupSystem.Instance.IsYesNoPopupShowing();
        }

        Debug.LogError("Error trying to call IsYesNoPopupShowing, UIYesNoPopupSystem is not initialized!");
        return false;
    }
}
#endif
