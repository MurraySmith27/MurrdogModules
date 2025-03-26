using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textElement;

    [SerializeField] private Animator animator;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform textBoxRectTransform;

    [SerializeField] private string animatorExpandUpTrigger = "Up";
    [SerializeField] private string animatorExpandDownTrigger = "Down";

    [SerializeField] private string animatorExitTrigger = "Exit";

    [Header("Position")] 
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private int verticalPaddingForMouse = 30;

    private bool _isExpandUp;

    private bool _isMouseOverTooltip;
    
    public void Populate(Vector2 anchorPos, string text)
    {
        AnimationUtils.ResetAnimator(animator, "None");
        
        textElement.SetText(text);

        rectTransform.anchoredPosition = anchorPos;

        Vector2 textBoxOffset;
        Vector2 textBoxPivot;

        float offsetAmount = 0;
        
        //anchor is at 0.5, 0.5, so -y means lower half
        if (anchorPos.y <= 0)
        {
            //expand upward
            animator.SetTrigger(animatorExpandUpTrigger);
            layoutGroup.padding.bottom += verticalPaddingForMouse;

            offsetAmount = layoutGroup.padding.bottom;
            
            textBoxOffset = new Vector2(0, -offsetAmount / 2f);

            textBoxPivot = new Vector2(0, 1);
            
            _isExpandUp = true;
        }
        else
        {
            //expand downward
            animator.SetTrigger(animatorExpandDownTrigger);
            layoutGroup.padding.top += verticalPaddingForMouse;
            
            offsetAmount = layoutGroup.padding.top;
            
            textBoxOffset = new Vector2(0, offsetAmount / 2f);
            
            textBoxPivot = new Vector2(0, 0);

            _isExpandUp = false;
        }

        if (anchorPos.x <= 0)
        {
            //expand right
            textBoxOffset = new Vector2(-offsetAmount / 2f, textBoxOffset.y);
            textBoxPivot = new Vector2(0, textBoxPivot.y);
        }
        else
        {
            //expand left
            textBoxOffset = new Vector2(offsetAmount / 2f, textBoxOffset.y);
            textBoxPivot = new Vector2(1, textBoxPivot.y);
        }

        textBoxRectTransform.pivot = textBoxPivot;
        textBoxRectTransform.anchoredPosition = textBoxOffset;
    }

    public void TearDown()
    {
        animator.SetTrigger(animatorExitTrigger);

        if (_isExpandUp)
        {
            layoutGroup.padding.bottom -= verticalPaddingForMouse;   
        }
        else
        {
            layoutGroup.padding.top -= verticalPaddingForMouse;
        }
    }

    public bool IsMouseOnTooltip()
    {
        return RectTransformUtils.IsMouseOverRectTransform(textBoxRectTransform);
    }
}
