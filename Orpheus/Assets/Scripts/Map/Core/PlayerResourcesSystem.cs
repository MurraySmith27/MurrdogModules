using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for tracking the player's owned resources
public class PlayerResourcesSystem : Singleton<PlayerResourcesSystem>
{
    public void AddResource(PersistentResourceType resourceType, int quantity)
    {
        ModifyResource(resourceType, quantity);
    }

    public void SpendResource(PersistentResourceType resourceType, int quantity)
    {
        ModifyResource(resourceType, -quantity);
    }

    private void ModifyResource(PersistentResourceType resourceType, int diff)
    {
        switch (resourceType)
        {
            case PersistentResourceType.Wood:
                RoundState.Instance.ChangeCurrentWood(diff);
                break;
            case PersistentResourceType.Stone:
                RoundState.Instance.ChangeCurrentStone(diff);
                break;
            case PersistentResourceType.Gold:
                RoundState.Instance.ChangeCurrentGold(diff);
                break;
        }
    }

    public bool HasResource(PersistentResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case PersistentResourceType.Wood:
                return RoundState.Instance.CurrentWood >= quantity;
            case PersistentResourceType.Stone:
                return RoundState.Instance.CurrentStone >= quantity;
            case PersistentResourceType.Gold:
                return RoundState.Instance.CurrentGold >= quantity;
        }

        return false;
    }

    public bool PayCost(List<PersistentResourceItem> costs)
    {
        foreach (PersistentResourceItem cost in costs)
        {
            if (!HasResource(cost.Type, cost.Quantity)) return false;
        }
        
        foreach (PersistentResourceItem cost in costs)
        {
            ModifyResource(cost.Type, -cost.Quantity);
        }

        return true;
    }
}
