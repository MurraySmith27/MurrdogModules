using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public BuildingType Type;
    public List<PersistentResourceItem> Costs = new List<PersistentResourceItem>();
    public List<TileType> CanBuildOnTiles = new List<TileType>();
}

[CreateAssetMenu(fileName = "BuildingsData", menuName = "Orpheus/BuildingsData", order = 1)]
public class BuildingsDataSO : ScriptableObject
{
    public List<PersistentResourceItem> DestroyBuildingsCost = new List<PersistentResourceItem>(new PersistentResourceItem[]{new PersistentResourceItem(PersistentResourceType.BuildToken, 1)});
    public List<BuildingData> Buildings = new List<BuildingData>();
    
}
