using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public abstract void SerializeItem();
    
    public virtual bool OnItemUsed(out AdditionalTriggeredArgs args)
    {
        args = new();
        return false;
    }

    public virtual bool IsItemUsable()
    {
        return false;
    }
}
