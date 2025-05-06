using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourceVisualData", menuName = "Orpheus/ResourceVisualData", order = 1)]
public class ResourceVisualDataSO : ScriptableObject
{
    [Serializable]
    private class ResourceVisualData
    {
        public ResourceType Type;
        public Sprite Sprite;
        public Color Color;
    }

    [SerializeField] private List<ResourceVisualData> visualData = new();

    public Sprite GetSpriteForResourceItem(ResourceType type)
    {
        ResourceVisualData data = visualData.FirstOrDefault((ResourceVisualData visData) =>
        {
            return visData.Type == type;
        });

        if (data == null)
        {
            Debug.LogError($"Cannot find visual data for resource of type: {Enum.GetName(typeof(ResourceType), type)}");
            return null;
        }
        else
        {
            return data.Sprite;
        }
    }
    
    public Color GetColorForResourceItem(ResourceType type)
    {
        ResourceVisualData data = visualData.FirstOrDefault((ResourceVisualData visData) =>
        {
            return visData.Type == type;
        });

        if (data == null)
        {
            Debug.LogError($"Cannot find visual data for resource of type: {Enum.GetName(typeof(ResourceType), type)}");
            return Color.clear;
        }
        else
        {
            return data.Color;
        }
    }
}
