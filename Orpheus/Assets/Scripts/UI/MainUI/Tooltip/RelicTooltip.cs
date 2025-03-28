using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RelicIcon))]
public class RelicTooltip : TooltipBase
{
    [SerializeField] private RelicVisualsSO relicVisuals;
    
    private RelicIcon _relicIcon;

    private void Awake()
    {
        _relicIcon = GetComponent<RelicIcon>();
    }
    
    protected override string GetTooltipText()
    {
        return relicVisuals.GetDescriptionForRelic(_relicIcon.GetRelicType());
    }
}
