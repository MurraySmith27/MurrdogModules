using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildngProcessRules.asset", menuName = "Orpheus/Building Process Rules", order = 1)]
public class BuildingProcessRulesSO : ScriptableObject
{
    [Serializable]
    public class BuildingProcessLane
    {
        public List<ResourceItem> ResourceInput;
        public List<PersistentResourceItem> PersistentResourceInput;

        public List<ResourceItem> ResourceOutput;
        public List<PersistentResourceItem> PersistentResourceOutput;
    }
    
    [Serializable]
    public class BuildingProcessRules
    {
        public BuildingType Type;

        public List<BuildingProcessLane> ProcessLanes;
    }
    
    [SerializeField] private List<BuildingProcessRules> buildingProcessRules;


    private BuildingProcessRules GetProcessRules(BuildingType buildingType)
    {
        return buildingProcessRules.Find(x => x.Type == buildingType);
    }

    public int GetNumProcessLanes(BuildingType buildingType)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        return rules.ProcessLanes.Count;
    }
    
    public List<int> GetLanesForWhichHasInputs(BuildingType buildingType, Dictionary<ResourceType, int> resources)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);
        
        List<int> lanes = new List<int>();
        Dictionary<ResourceType, int> resourcesUsedSoFar = new();
        Dictionary<PersistentResourceType, int> persistentResourceUsedSoFar = new();

        if (rules != null && HarvestState.IsAvailable)
        {
            int i = -1;
            foreach (BuildingProcessLane lane in rules.ProcessLanes)
            {
                i++;
                
                Dictionary<ResourceType, int> resourcesUsedThisLane = new();
                Dictionary<PersistentResourceType, int> persistentResourceUsedThisLane = new();
                
                bool satisfiesLaneInputs = true;
                foreach (ResourceItem resourceItem in lane.ResourceInput)
                {
                    int totalInputOwned = 0;
                    if (resources.ContainsKey(resourceItem.Type))
                        totalInputOwned += resources[resourceItem.Type];
                    
                    if (resourcesUsedThisLane.ContainsKey(resourceItem.Type))
                        totalInputOwned -= resourcesUsedThisLane[resourceItem.Type];
                    
                    if (totalInputOwned < resourceItem.Quantity)
                    {
                        satisfiesLaneInputs = false;
                        break;
                    }
                    else
                    {
                        if (!resourcesUsedThisLane.ContainsKey(resourceItem.Type))
                        {
                            resourcesUsedThisLane.Add(resourceItem.Type, 0);
                        }
                        
                        resourcesUsedThisLane[resourceItem.Type] += resourceItem.Quantity;
                    }
                }

                if (!satisfiesLaneInputs)
                {
                    continue;
                }
                
                foreach (PersistentResourceItem persistentResourceItem in lane.PersistentResourceInput)
                {
                    int totalRequiredAddition = 0;
                    
                    if (persistentResourceUsedThisLane.ContainsKey(persistentResourceItem.Type))
                        totalRequiredAddition += persistentResourceUsedThisLane[persistentResourceItem.Type];
                    
                    if (!PlayerResourcesSystem.Instance.HasResource(persistentResourceItem.Type,
                            persistentResourceItem.Quantity + totalRequiredAddition))
                    {
                        satisfiesLaneInputs = false;
                        break;
                    }
                    else
                    {
                        if (!persistentResourceUsedThisLane.ContainsKey(persistentResourceItem.Type))
                        {
                            persistentResourceUsedThisLane.Add(persistentResourceItem.Type, 0);
                        }
                        
                        persistentResourceUsedThisLane[persistentResourceItem.Type] += persistentResourceItem.Quantity;
                    }
                }
                
                if (!satisfiesLaneInputs)
                {
                    continue;
                }
                
                foreach (ResourceType type in resourcesUsedThisLane.Keys)
                {
                    if (!resourcesUsedSoFar.ContainsKey(type))
                    {
                        resourcesUsedSoFar.Add(type, 0);
                    }
                    
                    resourcesUsedSoFar[type] += resourcesUsedThisLane[type];
                }
                
                foreach (PersistentResourceType type in persistentResourceUsedThisLane.Keys)
                {
                    if (!persistentResourceUsedSoFar.ContainsKey(type))
                    {
                        persistentResourceUsedSoFar.Add(type, 0);
                    }
                    
                    persistentResourceUsedSoFar[type] += persistentResourceUsedThisLane[type];
                }

                lanes.Add(i);
            }
            
        }

        return lanes;
    }

    public List<ResourceItem> GetResourceInput(BuildingType buildingType, int laneIndex)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.ProcessLanes[laneIndex].ResourceInput;
        }
        else return new();
    }
    
    public List<PersistentResourceItem> GetPersistentResourceInput(BuildingType buildingType, int laneIndex)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.ProcessLanes[laneIndex].PersistentResourceInput;
        }
        else return new();
    }
    
    public List<ResourceItem> GetResourceOutput(BuildingType buildingType, int laneIndex)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.ProcessLanes[laneIndex].ResourceOutput;
        }
        else return new();
    }
    
    public List<PersistentResourceItem> GetPersistentResourceOutput(BuildingType buildingType, int laneIndex)
    {
        BuildingProcessRules rules = GetProcessRules(buildingType);

        if (rules != null)
        {
            return rules.ProcessLanes[laneIndex].PersistentResourceOutput;
        }
        else return new();
    }
}
