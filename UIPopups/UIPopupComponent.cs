using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIPopupComponent : MonoBehaviour
{
    [SerializeField] private Animator _animatorComponent;
    
    [SerializeField] private string _introAnimatorTrigger;

    [SerializeField] private string _outroAnimatorTrigger;
    
    private Coroutine _hidePopupCR;
    
    private bool _popupActionTriggeredThisFrame;

    private bool _hidePopupCRRunning;

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
        
        _animatorComponent.SetTrigger(_introAnimatorTrigger);
        IsActive = true;
    }


    public void OnPopupHide()
    {
        if (!IsActive || _popupActionTriggeredThisFrame)
        {
            return;
        }
        
        _popupActionTriggeredThisFrame = true;
        
        this.gameObject.SetActive(true);

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

        yield return new WaitUntil(() =>
        {
            return _animatorComponent.IsInTransition(0);
        });
        
        yield return new WaitUntil(() =>
        {
            return _animatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !_animatorComponent.IsInTransition(0);
        });
        
        this.gameObject.SetActive(false);
        _hidePopupCRRunning = false;
    }
}
