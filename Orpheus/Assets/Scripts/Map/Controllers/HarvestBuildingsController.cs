using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestBuildingsController : Singleton<HarvestBuildingsController>
{
    [SerializeField] private BuildingProcessRulesSO buildingProcessRules;
    
    public List<int> CanProcessLanes(BuildingType buildingType, Dictionary<ResourceType, int> resources)
    {
        return buildingProcessRules.GetLanesForWhichHasInputs(buildingType, resources);
    }

    public (List<ResourceItem>, List<PersistentResourceItem>) GetInputs(BuildingType buildingType, int laneIndex)
    {
        //assuming at this point that CanProcess has already been checked.
        List<ResourceItem> inputs = buildingProcessRules.GetResourceInput(buildingType, laneIndex);
        
        List<PersistentResourceItem> persistentInputs =
            buildingProcessRules.GetPersistentResourceInput(buildingType, laneIndex);
        
        return (inputs, persistentInputs);
    }
    
    public (List<ResourceItem>, List<PersistentResourceItem>) GetOutputs(BuildingType buildingType, int laneIndex)
    {
        //assuming at this point that CanProcess has already been checked.
        List<ResourceItem> outputs = buildingProcessRules.GetResourceOutput(buildingType, laneIndex);

        List<PersistentResourceItem> persistentOutputs =
            buildingProcessRules.GetPersistentResourceOutput(buildingType, laneIndex);
        
        return (outputs, persistentOutputs);
    }

    public (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) GetProcessBuildingDiff(BuildingType buildingType, int laneIndex)
    {
        Dictionary<ResourceType, int> diff = new();
        Dictionary<PersistentResourceType, int> persistentDiff = new();
        
        (List<ResourceItem>, List<PersistentResourceItem>) inputs = GetInputs(buildingType, laneIndex);

        foreach (ResourceItem input in inputs.Item1)
        {
            if (!diff.ContainsKey(input.Type))
            {
                diff.Add(input.Type, 0);
            }
            
            diff[input.Type] -= input.Quantity;
        }
        
        foreach (PersistentResourceItem input in inputs.Item2)
        {
            if (!persistentDiff.ContainsKey(input.Type))
            {
                persistentDiff.Add(input.Type, 0);
            }
            
            persistentDiff[input.Type] -= input.Quantity;
        }
        
        (List<ResourceItem>, List<PersistentResourceItem>) outputs = GetOutputs(buildingType, laneIndex);
        
        foreach (ResourceItem output in outputs.Item1)
        {
            if (!diff.ContainsKey(output.Type))
            {
                diff.Add(output.Type, 0);
            }
            
            diff[output.Type] += output.Quantity;
        }

        foreach (PersistentResourceItem output in outputs.Item2)
        {
            if (!persistentDiff.ContainsKey(output.Type))
            {
                persistentDiff.Add(output.Type, 0);
            }
            
            persistentDiff[output.Type] += output.Quantity;
        }

        return (diff, persistentDiff);

    }
}
