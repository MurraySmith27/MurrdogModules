using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildngProcessRules.asset", menuName = "Orpheus/Building Process Rules", order = 1)]
public class BuildingProcessRulesSO : ScriptableObject
{
    [Serializable]
    public class BuildingProcessRules
    {
        public BuildingType Type;
        
        public List<ResourceItem> ResourceInput;
        public List<PersistentResourceItem> PersistentResourceInput;

        public List<ResourceItem> ResourceOutput;
        public List<PersistentResourceItem> PersistentResourceOutput;
    }
    
    [SerializeField] private List<BuildingProcessRules> buildingProcessRules;


    private BuildingProcessRules GetProcessRules(BuildingType buildingType)
    {
        return buildingProcessRules.Find(x => x.Type == buildingType);
    }
    
    public bool HasInputs(BuildingType buildingType, Dictionary<ResourceType, int> resources)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null && HarvestState.IsAvailable)
        {
            foreach (ResourceItem resourceItem in rules.ResourceInput)
            {
                if (!resources.ContainsKey(resourceItem.Type) || resources[resourceItem.Type] < resourceItem.Quantity)
                {
                    return false;
                }
            }

            foreach (PersistentResourceItem persistentResourceItem in rules.PersistentResourceInput)
            {
                if (!PlayerResourcesSystem.Instance.HasResource(persistentResourceItem.Type,
                        persistentResourceItem.Quantity))
                {
                    return false;
                }
            }

            return true;
        }
        else return false;
    }

    public List<ResourceItem> GetResourceInput(BuildingType buildingType)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.ResourceInput;
        }
        else return new();
    }
    
    public List<PersistentResourceItem> GetPersistentResourceInput(BuildingType buildingType)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.PersistentResourceInput;
        }
        else return new();
    }
    
    public List<ResourceItem> GetResourceOutput(BuildingType buildingType)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.ResourceOutput;
        }
        else return new();
    }
    
    public List<PersistentResourceItem> GetPersistentResourceOutput(BuildingType buildingType)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.PersistentResourceOutput;
        }
        else return new();
    }
}
