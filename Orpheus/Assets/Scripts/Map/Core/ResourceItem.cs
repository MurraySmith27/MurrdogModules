using System;

[Serializable]
public class ResourceItem
{
    public ResourceType Type;
    public int Quantity;

    public ResourceItem(ResourceType type, int quantity)
    {
        Type = type;
        Quantity = quantity;
    }
}

[Serializable]
public class PersistentResourceItem
{
    public PersistentResourceType Type;
    public int Quantity;

    public PersistentResourceItem(PersistentResourceType type, int quantity)
    {
        Type = type;
        Quantity = quantity;
    }
}