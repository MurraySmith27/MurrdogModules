using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.Serialization;

public class UIPopupComponent : MonoBehaviour
{
    [SerializeField] private Animator _animatorComponent;
    
    [SerializeField] private string _introAnimatorTrigger;

    [SerializeField] private string _outroAnimatorTrigger;
    
    
    [Serializable]
    public class PopupHideStartEvent : UltEvent {}
    
    [Serializable]
    public class PopupShowFinishedEvent : UltEvent {}

    [SerializeField] private PopupHideStartEvent onPopupHideStart;
    
    [SerializeField] private PopupShowFinishedEvent onPopupShowFinished;
    
    private bool _popupActionTriggeredThisFrame;
    
    private Coroutine _hidePopupCR;

    private bool _hidePopupCRRunning;

    private Coroutine _showPopupCR;

    private bool _showPopupCRRunning;
    
    public bool IsActive
    {
        get;
        private set;
    }

    void Update()
    {
        _popupActionTriggeredThisFrame = false;
    }

    public void OnPopupShow()
    {
        if (IsActive || _popupActionTriggeredThisFrame)
        {
            return;
        }
        
        _popupActionTriggeredThisFrame = true;
        
        this.gameObject.SetActive(true);
        
        if (_hidePopupCR != null)
        {
            StopCoroutine(_hidePopupCR);
            _hidePopupCRRunning = false;
        }

        if (!_showPopupCRRunning)
        {
            _showPopupCR = StartCoroutine(OnPopupShowCR());
        }
    }

    private IEnumerator OnPopupShowCR()
    {
        _showPopupCRRunning = true;
        
        _animatorComponent.SetTrigger(_introAnimatorTrigger);
        IsActive = true;

        int beforeStateHash = _animatorComponent.GetCurrentAnimatorStateInfo(0).fullPathHash;
        
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = _animatorComponent.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 1 && stateInfo.fullPathHash != beforeStateHash;
        });
        
        onPopupShowFinished.Invoke();
        
        _showPopupCRRunning = false;
    }


    public void OnPopupHide()
    {
        if (!IsActive || _popupActionTriggeredThisFrame)
        {
            return;
        }
        
        _popupActionTriggeredThisFrame = true;
        
        this.gameObject.SetActive(true);
        
        if (_showPopupCR != null)
        {
            StopCoroutine(_showPopupCR);
            onPopupShowFinished.Invoke();
            _showPopupCRRunning = false;
        }

        if (!_hidePopupCRRunning)
        {
            _hidePopupCR = StartCoroutine(OnPopupHideCR());
        }
    }

    private IEnumerator OnPopupHideCR()
    {
        _hidePopupCRRunning = true;
        
        _animatorComponent.SetTrigger(_outroAnimatorTrigger);
        IsActive = false;
        
        onPopupHideStart.Invoke();

        yield return new WaitUntil(() =>
        {
            return _animatorComponent.IsInTransition(0);
        });
        
        int beforeStateHash = _animatorComponent.GetCurrentAnimatorStateInfo(0).fullPathHash;
        
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = _animatorComponent.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 1 && stateInfo.fullPathHash != beforeStateHash;
        });
        
        this.gameObject.SetActive(false);
        _hidePopupCRRunning = false;
    }
}
