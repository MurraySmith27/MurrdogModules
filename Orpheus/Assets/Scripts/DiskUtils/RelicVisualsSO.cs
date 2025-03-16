using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RelicVisualsData", menuName = "Orpheus/Relic Visuals Data", order = 1)]
public class RelicVisualsSO : ScriptableObject
{
    [Serializable]
    public struct RelicTypeToSprite
    {
        public RelicTypes relicType;
        public Sprite sprite;
    }
    
    [SerializeField] private List<RelicTypeToSprite> relicTypes = new List<RelicTypeToSprite>(); 

    public Sprite GetIconForRelic(RelicTypes relicType)
    {
        RelicTypeToSprite relicTypeToSprite = relicTypes.Find(x => x.relicType == relicType);

        return relicTypeToSprite.sprite;
    }
}
