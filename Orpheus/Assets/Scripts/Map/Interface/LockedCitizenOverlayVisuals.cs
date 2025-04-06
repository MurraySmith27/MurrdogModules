using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedCitizenOverlayVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorEnterTrigger = "Enter";
    [SerializeField] private string animatorExitTrigger = "Enter";
    [SerializeField] private string animatorSetLockedTrigger = "Lock";
    [SerializeField] private string animatorSetUnlockedTrigger = "Unlock";
    
    public void Show()
    {
        animator.SetTrigger(animatorEnterTrigger);
    }

    public void Hide()
    {
        animator.SetTrigger(animatorExitTrigger);
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
}
