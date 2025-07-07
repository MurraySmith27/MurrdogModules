using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ResourceTags
{
    FOOD,
    INDUSTRIAL,
    DAIRY,
    BAKED,
}

[Serializable]
public class ResourceTagMapping
{
    public ResourceType Type;

    public List<ResourceTags> Tags;
}

[CreateAssetMenu(menuName = "Orpheus/Resource Tags SO", fileName = "ResourceTagsSO", order = 0)]
public class ResourceTagsSO : ScriptableObject
{
    [SerializeField] private List<ResourceTagMapping> ResourceTagMappings;

    public List<ResourceTags> GetAllTagsForResource(ResourceType type)
    {
        ResourceTagMapping tagMapping = ResourceTagMappings.FirstOrDefault((ResourceTagMapping mapping) =>
        {
            return mapping.Type == type;
        });

        if (tagMapping != null)
        {
            return tagMapping.Tags;
        }
        else
        {
            Debug.LogError($"No data for resource type: {Enum.GetName(typeof(ResourceType),type)} exists in ResourceTagsSO!");
            return new();
        }
    }

    public bool DoesResourceHaveTag(ResourceType type, ResourceTags tag)
    {
        List<ResourceTags> resourceTagsList = GetAllTagsForResource(type);

        return resourceTagsList.Contains(tag);
    }
}
