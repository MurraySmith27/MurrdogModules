using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourceVisualData", menuName = "Orpheus/ResourceVisualData", order = 1)]
public class ResourceVisualDataSO : ScriptableObject
{
    [SerializeField] private Texture2D cornIcon;
    [SerializeField] private Texture2D wheatIcon;
    [SerializeField] private Texture2D fishIcon;
    [SerializeField] private Texture2D woodIcon;
    [SerializeField] private Texture2D stoneIcon;


    public Texture2D GetTextureForResourceItem(ResourceItem resourceItem)
    {

        switch (resourceItem.Type)
        {
            case ResourceType.Corn:
                return cornIcon;
            case ResourceType.Wheat:
                return wheatIcon;
            case ResourceType.Fish:
                return fishIcon;
            case ResourceType.Wood:
                return woodIcon;
            case ResourceType.Stone:
                return stoneIcon;
        }

        return null;
    }
}
