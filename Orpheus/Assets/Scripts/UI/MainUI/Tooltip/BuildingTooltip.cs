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
        string basicDescription = buildingVisualsSO.GetDescriptionForBuilding(buildingType);

        string additionalDescription = string.IsNullOrEmpty(basicDescription) ? "" : "\n";

        var input = buildingProcessRulesSO.GetResourceInput(buildingType);


        if (input.Count > 0)
        {
            foreach (ResourceItem resourceItem in input)
            {
                additionalDescription += LocalizationUtils.GetIconTagForResource(resourceItem.Type) + (resourceItem.Quantity > 1 ? resourceItem.Quantity.ToString() : "");
            }
            additionalDescription += " > ";
        }
        else
        {
            additionalDescription += "Harvests ";
        }

        var output = buildingProcessRulesSO.GetResourceOutput(buildingType);
        
        foreach (ResourceItem resourceItem in output)
        {
            additionalDescription += LocalizationUtils.GetIconTagForResource(resourceItem.Type) + (resourceItem.Quantity > 1 ? resourceItem.Quantity.ToString() : "");
        }

        var persistentInput = buildingProcessRulesSO.GetPersistentResourceInput(buildingType);

        if (persistentInput.Count > 0)
        {
            additionalDescription += "\nCosts: ";
                
            foreach (PersistentResourceItem persistentResourceItem in persistentInput)
            {
                additionalDescription += LocalizationUtils.GetIconTagForPersistentResource(persistentResourceItem.Type) + persistentResourceItem.Quantity.ToString();
            }
        }
        
        return basicDescription + additionalDescription;
    }
}
