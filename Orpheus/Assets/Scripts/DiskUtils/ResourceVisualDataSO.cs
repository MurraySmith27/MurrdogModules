using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourceVisualData", menuName = "Orpheus/ResourceVisualData", order = 1)]
public class ResourceVisualDataSO : ScriptableObject
{
    [SerializeField] private Sprite cornIcon;
    [SerializeField] private Sprite wheatIcon;
    [SerializeField] private Sprite fishIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite stoneIcon;


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
        }

        return null;
    }
}
