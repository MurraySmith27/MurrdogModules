using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMouseData : Singleton<UIMouseData>
{
    [SerializeField] private OrpheusUIInputChannel inputChannel;
    
    private List<RectTransform> currentMouseOverRectTransforms = new();

    private void Start()
    {
        inputChannel.MouseMoveEvent -= OnMouseMove;
        inputChannel.MouseMoveEvent += OnMouseMove;
    }

    private void OnDestroy()
    {
        if (inputChannel != null)
        {
            inputChannel.MouseMoveEvent -= OnMouseMove;
        }
    }
    
    public void OnMouseMove(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        currentMouseOverRectTransforms = RectTransformUtils.GetMouseOverRectTransforms();
    }

    public bool IsMouseOverRectTransform(RectTransform rectTransform)
    {
        return currentMouseOverRectTransforms.Contains(rectTransform);
    }
}
