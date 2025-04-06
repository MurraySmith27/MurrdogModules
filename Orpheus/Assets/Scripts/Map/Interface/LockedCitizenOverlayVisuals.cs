using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedCitizenOverlayVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorEnterTrigger = "Enter";
    [SerializeField] private string animatorExitTrigger = "Exit";
    [SerializeField] private string animatorSetLockedTrigger = "Lock";
    [SerializeField] private string animatorSetUnlockedTrigger = "Unlock";

    [SerializeField] private float exitWaitTime = 2f;

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
        animator.gameObject.SetActive(true);
        AnimationUtils.ResetAnimator(animator);
        animator.SetTrigger(animatorEnterTrigger);
    }

    public void Hide()
    {
        animator.SetTrigger(animatorExitTrigger);

        OrpheusTiming.InvokeAfterSecondsGameTime(exitWaitTime, () =>
        {
            animator.gameObject.SetActive(false);
        });
    }

    public void SetLock(bool locked)
    {
        if (locked)
        {
            animator.SetTrigger(animatorSetLockedTrigger);
        }
        else
        {
            animator.SetTrigger(animatorSetUnlockedTrigger);
        }
    }

    private void OnGameSpeedChanged(float gameSpeed)
    {
        animator.speed = gameSpeed;
    }
}
