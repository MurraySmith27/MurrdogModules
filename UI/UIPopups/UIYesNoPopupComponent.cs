using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIYesNoPopupComponent : MonoBehaviour
{
    [Serializable]
    public class PromptTextChangedEvent : UnityEvent<string> {}

    [SerializeField] private PromptTextChangedEvent _onPromptTextChanged;
    [SerializeField] private TMP_Text _text;
    
    private Action _onYesCallback;
    private Action _onNoCallback;

    public void Configure(string promptText, Action onYes, Action onNo)
    {
        _text.SetText(promptText);
        _onPromptTextChanged?.Invoke(promptText);
        _onYesCallback = onYes;
        _onNoCallback = onNo;
    }

    public void OnYesPressed()
    {
        _onYesCallback?.Invoke();
    }

    public void OnNoPressed()
    {
        _onNoCallback?.Invoke();
    }
}
