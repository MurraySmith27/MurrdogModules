using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for tracking the player's owned resources
public class PlayerResourcesSystem : Singleton<PlayerResourcesSystem>
{
    private int _wood = 0;
    
    public int Wood
    {
        get
        {
            //TODO: add serialization and save file
            return _wood;
        }
        set
        {
            _wood = value;
        }
    }
    
    private int _stone = 0;
    
    public int Stone
    {
        get
        {
            //TODO: add serialization and save file
            return _stone;
        }
        set
        {
            _stone = value;
        }
    }
    
    private int _gold = 0;
    
    public int Gold
    {
        get
        {
            //TODO: add serialization and save file
            return _gold;
        }
        set
        {
            _gold = value;
        }
    }

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
                Wood += diff;
                break;
            case PersistentResourceType.Stone:
                Stone += diff;
                break;
            case PersistentResourceType.Gold:
                Gold += diff;
                break;
        }
    }

    public bool HasResource(PersistentResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case PersistentResourceType.Wood:
                return Wood >= quantity;
            case PersistentResourceType.Stone:
                return Stone >= quantity;
            case PersistentResourceType.Gold:
                return Gold >= quantity;
            
        }

        return false;
    }
}
