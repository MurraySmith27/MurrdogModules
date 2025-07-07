using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : Singleton<ResourcesController>
{
    [SerializeField] private ResourceTagsSO resourceTagsSO;

    public bool DoesResourceHaveTag(ResourceType type, ResourceTags tag)
    {
        return resourceTagsSO.DoesResourceHaveTag(type, tag);
    } 
}
