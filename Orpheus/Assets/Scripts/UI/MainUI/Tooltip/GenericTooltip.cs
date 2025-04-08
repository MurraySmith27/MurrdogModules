using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTooltip : TooltipBase
{
    [SerializeField] private TooltipTextSO tooltipTextData;
    
    [SerializeField] private TooltipTextType tooltipType;
    
    protected override string GetTooltipText()
    {
        return tooltipTextData.GetTooltipText(tooltipType);
    }
}
