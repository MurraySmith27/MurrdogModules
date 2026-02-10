#if USING_NEW_INPUT_SYSTEM
using System;
using UnityEngine;

public class UIYesNoPopupSystem : Singleton<UIYesNoPopupSystem>
{
    [SerializeField] private string _yesNoPopupId;
    [SerializeField] private UIYesNoPopupComponent _yesNoPopupComponent;

    private UIYesNoPromptType _currentPromptType;

    public event Action<UIYesNoPromptType> OnYesNoPopupRequested;
    public event Action<UIYesNoPromptType, bool> OnYesNoPopupAnswered;

    public UIYesNoPromptType CurrentPromptType => _currentPromptType;

    public void RequestYesNoPopup(UIYesNoPromptType promptType, string promptText, Action onYes, Action onNo,
        PanelShowBehaviour showBehaviour = PanelShowBehaviour.HIDE_PREVIOUS)
    {
        _currentPromptType = promptType;

        _yesNoPopupComponent.Configure(promptText,
            () =>
            {
                onYes?.Invoke();
                OnYesNoPopupAnswered?.Invoke(promptType, true);
                UIPopupSystem.Instance.HidePopup(_yesNoPopupId);
            },
            () =>
            {
                onNo?.Invoke();
                OnYesNoPopupAnswered?.Invoke(promptType, false);
                UIPopupSystem.Instance.HidePopup(_yesNoPopupId);
            }
        );

        UIPopupSystem.Instance.ShowPopup(_yesNoPopupId, showBehaviour);
        OnYesNoPopupRequested?.Invoke(promptType);
    }

    public void DismissYesNoPopup()
    {
        UIPopupSystem.Instance.HidePopup(_yesNoPopupId);
    }

    public bool IsYesNoPopupShowing()
    {
        return UIPopupSystem.Instance.IsPopupShowing(_yesNoPopupId);
    }
}
#endif
