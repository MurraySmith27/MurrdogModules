using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationUtils
{
    public static void ResetAnimator(Animator animator, string defaultStateName)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
        
        animator.Play(defaultStateName);
    }
}
