using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourceVisualData", menuName = "Orpheus/ResourceVisualData", order = 1)]
public class ResourceVisualDataSO : ScriptableObject
{
    [SerializeField] private Sprite cornIcon;
    [SerializeField] private Sprite wheatIcon;
    [SerializeField] private Sprite fishIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite stoneIcon;
    [SerializeField] private Sprite breadIcon;
    [SerializeField] private Sprite lumberIcon;
    [SerializeField] private Sprite copperIcon;
    [SerializeField] private Sprite steelIcon;
    [SerializeField] private Sprite popcornIcon;
    [SerializeField] private Sprite sushiIcon;

    public Sprite GetSpriteForResourceItem(ResourceType type)
    {
        switch (type)
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
            case ResourceType.Bread:
                return breadIcon;
            case ResourceType.Lumber:
                return lumberIcon;
            case ResourceType.Copper:
                return copperIcon;
            case ResourceType.Steel:
                return steelIcon;
            case ResourceType.Popcorn:
                return popcornIcon;
            case ResourceType.Sushi:
                return sushiIcon;
        }

        return null;
    }
}
