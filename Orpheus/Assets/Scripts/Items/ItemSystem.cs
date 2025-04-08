using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    NONE,
    BONUS_CITIZEN
}

public class ItemSystem : Singleton<ItemSystem>
{
    private Dictionary<ItemTypes, Item> _itemInstances = new Dictionary<ItemTypes, Item>();
    
    private List<ItemTypes> items = new List<ItemTypes>();
    
    public event Action<ItemTypes> OnItemAdded;
    public event Action<ItemTypes> OnItemRemoved;
    public event Action<ItemTypes, AdditionalTriggeredArgs> OnItemUsed;
    public event Action<ItemTypes> OnItemDiscarded;
    
    private void Awake()
    {
        ItemFactory itemFactory = new ItemFactory();
        
        for (int i = 0; i < Enum.GetValues(typeof(ItemTypes)).Length; i++)
        {
            _itemInstances.Add((ItemTypes)i, itemFactory.CreateItem((ItemTypes)i));
        }
    }
    
    public void AddItem(ItemTypes item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            OnItemAdded?.Invoke(item);
        }
    }

    public void RemoveItem(ItemTypes item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            OnItemRemoved?.Invoke(item);
        }
    }

    public void UseItem(ItemTypes item)
    {
        if (items.Contains(item))
        {
            RemoveItem(item);

            AdditionalTriggeredArgs args;
            _itemInstances[item].OnItemUsed(out args);
            
            OnItemUsed?.Invoke(item, args);
        }
    }
    
    public void DiscardItem(ItemTypes item)
    {
        if (items.Contains(item))
        {
            RemoveItem(item);
            OnItemDiscarded?.Invoke(item);
        }
    }
    
    public bool HasItem(ItemTypes item)
    {
        return items.Contains(item);
    }
    
    public List<ItemTypes> GetOwnedItems()
    {
        return items;
    }
}
