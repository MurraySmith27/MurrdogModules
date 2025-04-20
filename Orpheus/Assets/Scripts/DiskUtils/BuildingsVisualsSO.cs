using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsVisualsData", menuName = "Orpheus/Buildings Visuals Data", order = 1)]
public class BuildingsVisualsSO : ScriptableObject
{
    public List<BuildingVisualsData> BuildingsVisualsData = new List<BuildingVisualsData>();
}

[Serializable]
public class BuildingVisualsData
{
    public BuildingType Type;
    public BuildingBehaviour Prefab;
    public BuildingBehaviour Icon3DPrefab;
}
