using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitizenLockIcon : MonoBehaviour
{
    [SerializeField] private bool aimAtCamera = true;

    [SerializeField] private Animator animator;

    [SerializeField] private string animatorDisappearTriggerName = "Exit";
    [SerializeField] private string animatorAppearTriggerName = "Enter";
    [SerializeField] private string animatorLockTriggerName = "Lock";
    [SerializeField] private string animatorUnlockTriggerName = "Unlock";

    [SerializeField] private float disappearWaitTime = 0.5f;

    private Camera _mainCamera;

    private bool _locked = false;

    private void OnEnable()
    {
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
        GlobalSettings.OnGameSpeedChanged += OnGameSpeedChanged;

        OnGameSpeedChanged(GlobalSettings.GameSpeed);
    }

    private void OnDisable()
    {
        GlobalSettings.OnGameSpeedChanged -= OnGameSpeedChanged;
    }
    
    public void Show()
    {
        this.gameObject.SetActive(true);
        
        AnimationUtils.ResetAnimator(animator);
        
        animator.SetTrigger(animatorAppearTriggerName);
        //start locked
        ToggleLocked(true);
    }

    public void Hide(Action onHideComplete = null)
    {
        animator.SetTrigger(animatorDisappearTriggerName);
        OrpheusTiming.InvokeAfterSecondsGameTime(disappearWaitTime, () =>
        {
            this.gameObject.SetActive(false);
            
            onHideComplete?.Invoke();
        } );
    }
    
    public void ToggleLocked(bool locked)
    {
        if (locked != _locked)
        {
            if (locked)
            {
                animator.SetTrigger(animatorLockTriggerName);
            }
            else
            {
                animator.SetTrigger(animatorUnlockTriggerName);
            }
        }
        
        _locked = locked;
    }
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (aimAtCamera) {
            //reorient to look at the camera
            transform.localRotation = Quaternion.Euler(Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up).eulerAngles.x - 90, 0, 0);
        }
    }

    private void OnGameSpeedChanged(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }
}