using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for tracking the player's owned resources
public class PlayerResourcesSystem : Singleton<PlayerResourcesSystem>
{
    private int _food = 0;
    
    public int Food
    {
        get
        {
            //TODO: add serialization and save file
            return _food;
        }
        set
        {
            _food = value;
        }
    }

    public void AddResource(ResourceType resourceType, int quantity)
    {
        ModifyResource(resourceType, quantity);
    }

    public void SpendResource(ResourceType resourceType, int quantity)
    {
        ModifyResource(resourceType, -quantity);
    }

    private void ModifyResource(ResourceType resourceType, int diff)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                Food += diff;
                break;
        }
    }

    public bool HasResource(ResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                return Food >= quantity;
        }

        return false;
    }
}
