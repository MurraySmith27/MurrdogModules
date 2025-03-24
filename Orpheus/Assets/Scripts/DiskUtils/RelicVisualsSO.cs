using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RelicVisualsData", menuName = "Orpheus/Relic Visuals Data", order = 1)]
public class RelicVisualsSO : ScriptableObject
{
    [Serializable]
    public struct RelicTypeToVisualData
    {
        public RelicTypes relicType;
        public Sprite sprite;
        public GameObject visualsPrefab;
    }
    
    [SerializeField] private List<RelicTypeToVisualData> relicTypes = new List<RelicTypeToVisualData>(); 

    public Sprite GetIconForRelic(RelicTypes relicType)
    {
        RelicTypeToVisualData relicTypeToVisualData = relicTypes.Find(x => x.relicType == relicType);

        return relicTypeToVisualData.sprite;
    }

    public GameObject GetVisualsPrefabForRelic(RelicTypes relicType)
    {
        RelicTypeToVisualData relicTypeToVisualData = relicTypes.Find(x => x.relicType == relicType);

        return relicTypeToVisualData.visualsPrefab;
    }
}
