using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTooltip : TooltipBase
{
    [SerializeField] private BuildingsVisualsSO buildingVisualsSO;

    [SerializeField] private BuildingProcessRulesSO buildingProcessRulesSO;
    
    private BuildingIcon _buildingIcon;

    private void Awake()
    {
        _buildingIcon = GetComponentInChildren<BuildingIcon>();
    }

    protected override string GetTooltipText()
    {
        BuildingType buildingType = _buildingIcon.GetBuildingType();
        return LocalizationUtils.GetDescriptionOfBuilding(buildingType, buildingVisualsSO, buildingProcessRulesSO);
    }
}
