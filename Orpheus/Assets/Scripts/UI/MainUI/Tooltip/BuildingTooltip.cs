using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTooltip : TooltipBase
{
    [SerializeField] private BuildingsVisualsSO buildingVisualsSO;
    
    private BuildingIcon _buildingIcon;

    private void Awake()
    {
        _buildingIcon = GetComponentInChildren<BuildingIcon>();
    }
    
    protected override string GetTooltipText()
    {
        return buildingVisualsSO.GetDescriptionForBuilding(_buildingIcon.GetBuildingType());
    }
}
