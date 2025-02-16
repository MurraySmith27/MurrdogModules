using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public BuildingType Type;
    public List<ResourceItem> Costs = new List<ResourceItem>();
    public List<TileType> CanBuildOnTiles = new List<TileType>();
}

[CreateAssetMenu(fileName = "BuildingsData", menuName = "Orpheus/BuildingsData", order = 1)]
public class BuildingsDataSO : ScriptableObject
{
    public List<BuildingData> Buildings = new List<BuildingData>();
}
