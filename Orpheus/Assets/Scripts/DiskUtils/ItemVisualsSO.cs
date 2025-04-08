using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemVisualsData", menuName = "Orpheus/Item Visuals Data", order = 1)]
public class ItemVisualsSO : ScriptableObject
{
    [Serializable]
    public struct ItemTypeToVisualData
    {
        public ItemTypes itemType;
        public Sprite sprite;
        public GameObject visualsPrefab;
        public string description;
    }
    
    [SerializeField] private List<ItemTypeToVisualData> itemTypes = new List<ItemTypeToVisualData>(); 

    public Sprite GetIconForItem(ItemTypes itemType)
    {
        ItemTypeToVisualData itemTypeToVisualData = itemTypes.Find(x => x.itemType == itemType);

        return itemTypeToVisualData.sprite;
    }

    public GameObject GetVisualsPrefabForItem(ItemTypes itemType)
    {
        ItemTypeToVisualData itemTypeToVisualData = itemTypes.Find(x => x.itemType == itemType);

        return itemTypeToVisualData.visualsPrefab;
    }

    public string GetDescriptionForItem(ItemTypes itemType)
    {
        ItemTypeToVisualData itemTypeToVisualData = itemTypes.Find(x => x.itemType == itemType);

        return itemTypeToVisualData.description;
    }
}
