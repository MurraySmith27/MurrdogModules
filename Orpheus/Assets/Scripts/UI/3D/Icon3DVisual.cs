using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon3DVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void Start()
    {
        animator.Play("Enter");
    }

    public void Disappear()
    {
        animator.SetTrigger("Exit");
    }
}
