using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ResourceIconRetriever : Singleton<ResourceIconRetriever>
{
    [SerializeField] private ResourceIconObjectPool cornIconsObjectPool;
    [SerializeField] private ResourceIconObjectPool wheatIconsObjectPool;
    [SerializeField] private ResourceIconObjectPool fishIconsObjectPool;
    [SerializeField] private ResourceIconObjectPool woodIconsObjectPool;
    [SerializeField] private ResourceIconObjectPool stoneIconsObjectPool;

    public Transform GetResourceIcon(ResourceItem resourceItem)
    {
        switch (resourceItem.Type)
        {
            case ResourceType.Corn:
                return cornIconsObjectPool.GetIcon(resourceItem.Quantity);
            case ResourceType.Wheat:
                return wheatIconsObjectPool.GetIcon(resourceItem.Quantity);
            case ResourceType.Fish:
                return fishIconsObjectPool.GetIcon(resourceItem.Quantity);
            case ResourceType.Wood:
                return woodIconsObjectPool.GetIcon(resourceItem.Quantity);
            case ResourceType.Stone:
                return stoneIconsObjectPool.GetIcon(resourceItem.Quantity);
            default:
                Debug.LogError($"No such object pool exists for resource of type: {resourceItem.Type}.");
                return null;
        }
    }

    public void ReturnResourceIcon(ResourceItem item, Transform resourceIcon)
    {
        switch (item.Type)
        {
            case ResourceType.Corn:
                cornIconsObjectPool.ReturnIcon(resourceIcon, item.Quantity);
                break;
            case ResourceType.Wheat:
                wheatIconsObjectPool.ReturnIcon(resourceIcon, item.Quantity);
                break;
            case ResourceType.Fish:
                fishIconsObjectPool.ReturnIcon(resourceIcon, item.Quantity);
                break;
            case ResourceType.Wood:
                woodIconsObjectPool.ReturnIcon(resourceIcon, item.Quantity);
                break;
            case ResourceType.Stone:
                stoneIconsObjectPool.ReturnIcon(resourceIcon, item.Quantity);
                break;
        }
    }
}
