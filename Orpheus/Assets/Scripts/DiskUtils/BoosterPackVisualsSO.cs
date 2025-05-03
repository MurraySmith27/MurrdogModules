using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "BoosterPackVisualsData", menuName = "Orpheus/Booster Pack Visuals Data", order = 1)]
public class BoosterPackVisualsSO : ScriptableObject
{
    [Serializable]
    public struct BoosterPackTypeToVisualsData
    {
        public BoosterPackTypes boosterPackType;
        public Sprite sprite;
        public GameObject visualsPrefab;
        public string description;
    }
    
    [SerializeField] private List<BoosterPackTypeToVisualsData> itemTypes = new List<BoosterPackTypeToVisualsData>(); 

    public Sprite GetIconForBoosterPack(BoosterPackTypes boosterPackType)
    {
        BoosterPackTypeToVisualsData itemTypeToVisualData = itemTypes.Find(x => x.boosterPackType == boosterPackType);

        return itemTypeToVisualData.sprite;
    }

    public GameObject GetVisualsPrefabForBoosterPack(BoosterPackTypes boosterPackType)
    {
        BoosterPackTypeToVisualsData itemTypeToVisualData = itemTypes.Find(x => x.boosterPackType == boosterPackType);

        return itemTypeToVisualData.visualsPrefab;
    }

    public string GetDescriptionForBoosterPack(BoosterPackTypes boosterPackType)
    {
        BoosterPackTypeToVisualsData itemTypeToVisualData = itemTypes.Find(x => x.boosterPackType == boosterPackType);

        return itemTypeToVisualData.description;
    }
}