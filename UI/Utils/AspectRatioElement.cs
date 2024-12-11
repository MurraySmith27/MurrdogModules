using UnityEngine;
using UnityEngine.UIElements;

public partial class AspectRatioElement : VisualElement
{
    public int RatioWidth
    {
        get => _ratioWidth;
        set
        {
            _ratioWidth = value;
            UpdateAspect();
        }
    }
    
    public int RatioHeight
    {
        get => _ratioHeight;
        set
        {
            _ratioHeight = value;
            UpdateAspect();
        }
    }
    
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        // The progress property is exposed to UXML.
        UxmlFloatAttributeDescription m_ProgressAttribute = new UxmlFloatAttributeDescription()
        {
            name = "progress"
        };

        // Use the Init method to assign the value of the progress UXML attribute to the C# progress property.
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
        }
    }
    
    public new class UxmlFactory : UxmlFactory<AspectRatioElement, UxmlTraits> { }
    
    // Padding elements to keep the aspect ratio.
    private int _ratioWidth = 1;
    private int _ratioHeight = 1;
    
    public AspectRatioElement()
    {
        // Update the padding elements when the geometry changes.
        RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
        // Update the padding elements when the element is attached to a panel.
        RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
    }
    
    static void UpdateAspectAfterEvent(EventBase evt)
    {
        var element = evt.target as AspectRatioElement;
        element?.UpdateAspect();
    }
    
    private void ClearPadding()
    {
        style.paddingLeft = 0;
        style.paddingRight = 0;
        style.paddingBottom = 0;
        style.paddingTop = 0;
    }
        
    // Update the padding.
    private void UpdateAspect()
    {
        var designRatio = (float)RatioWidth / RatioHeight;
        var currRatio = resolvedStyle.width / resolvedStyle.height;
        var diff = currRatio - designRatio;
            
        if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
        {
            ClearPadding();
            Debug.LogError($"[AspectRatio] Invalid width:{RatioWidth} or height:{RatioHeight}");
            return;
        }
    
        if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
        {
            return;
        }
            
        if (diff > 0.01f)
        {
            var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
            style.paddingLeft = w;
            style.paddingRight = w;
            style.paddingTop = 0;
            style.paddingBottom = 0;
        }
        else if (diff < -0.01f)
        {
            var h = (resolvedStyle.height - (resolvedStyle.width * (1/designRatio))) * 0.5f;
            style.paddingLeft= 0;
            style.paddingRight = 0;
            style.paddingTop = h;
            style.paddingBottom = h;
        }
        else
        {
            ClearPadding();
        }
    }
}