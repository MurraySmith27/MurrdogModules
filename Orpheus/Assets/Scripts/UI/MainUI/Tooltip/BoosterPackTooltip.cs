using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoosterPackIcon))]
public class BoosterPackTooltip : TooltipBase
{
    [SerializeField] private BoosterPackVisualsSO boosterPackVisuals;
    
    private BoosterPackIcon _boosterPackIcon;

    private void Awake()
    {
        _boosterPackIcon = GetComponent<BoosterPackIcon>();
    }
    
    protected override string GetTooltipText()
    {
        return boosterPackVisuals.GetDescriptionForBoosterPack(_boosterPackIcon.GetBoosterPackType());
    }
}
