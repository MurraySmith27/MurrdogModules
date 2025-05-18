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

        int numLanes = buildingProcessRulesSO.GetNumProcessLanes(buildingType);

        for (int i = 0; i < numLanes; i++)
        {

            var input = buildingProcessRulesSO.GetResourceInput(buildingType, i);

            var output = buildingProcessRulesSO.GetResourceOutput(buildingType, i);

            if (input.Count > 0)
            {
                foreach (ResourceItem resourceItem in input)
                {
                    additionalDescription += LocalizationUtils.GetIconTagForResource(resourceItem.Type) +
                                             (resourceItem.Quantity > 1 ? resourceItem.Quantity.ToString() : "");
                }

                additionalDescription += " > ";
            }
            else if (output.Count > 0)
            {
                additionalDescription += "Harvests ";
            }

            foreach (ResourceItem resourceItem in output)
            {
                additionalDescription += LocalizationUtils.GetIconTagForResource(resourceItem.Type) +
                                         (resourceItem.Quantity > 1 ? resourceItem.Quantity.ToString() : "");
            }


            var persistentOutput = buildingProcessRulesSO.GetPersistentResourceOutput(buildingType, i);

            if (persistentOutput.Count > 0)
            {
                additionalDescription += "\nGrants ";

                foreach (PersistentResourceItem persistentResourceItem in persistentOutput)
                {
                    additionalDescription +=
                        LocalizationUtils.GetIconTagForPersistentResource(persistentResourceItem.Type) +
                        persistentResourceItem.Quantity.ToString();
                }
            }


            var persistentInput = buildingProcessRulesSO.GetPersistentResourceInput(buildingType, i);

            if (persistentInput.Count > 0)
            {
                additionalDescription += "\nCosts ";

                foreach (PersistentResourceItem persistentResourceItem in persistentInput)
                {
                    additionalDescription +=
                        LocalizationUtils.GetIconTagForPersistentResource(persistentResourceItem.Type) +
                        persistentResourceItem.Quantity.ToString();
                }
            }

        }

        string description = basicDescription + additionalDescription;

        if (description.StartsWith("\n"))
        {
            description = description.Substring(1);
        }
        
        return $"<b>{LocalizationUtils.GetNameOfBuilding(buildingType)}</b>\n{description}";
    }
}
