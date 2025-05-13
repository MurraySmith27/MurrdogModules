using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsVisualsData", menuName = "Orpheus/Buildings Visuals Data", order = 1)]
public class BuildingsVisualsSO : ScriptableObject
{
    public List<BuildingVisualsData> BuildingsVisualsData = new List<BuildingVisualsData>();

    public string GetDescriptionForBuilding(BuildingType buildingType)
    {
        BuildingVisualsData buildingVisualsData = BuildingsVisualsData.FirstOrDefault((BuildingVisualsData data) =>
        {
            return data.Type == buildingType;
        });

        if (buildingVisualsData != null)
        {
            return buildingVisualsData.Descrpition;
        }

        return "";
    }

    public BuildingBehaviour GetIcon3dPrefabForBuilding(BuildingType buildingType)
    {
        BuildingVisualsData buildingVisualsData = BuildingsVisualsData.FirstOrDefault((BuildingVisualsData data) =>
        {
            return data.Type == buildingType;
        });

        if (buildingVisualsData != null)
        {
            return buildingVisualsData.Icon3DPrefab;
        }

        return null;
    }
    
    public BuildingBehaviour GetPrefabForBuilding(BuildingType buildingType)
    {
        BuildingVisualsData buildingVisualsData = BuildingsVisualsData.FirstOrDefault((BuildingVisualsData data) =>
        {
            return data.Type == buildingType;
        });

        if (buildingVisualsData != null)
        {
            return buildingVisualsData.Prefab;
        }

        return null;
    }
}

[Serializable]
public class BuildingVisualsData
{
    public BuildingType Type;
    public BuildingBehaviour Prefab;
    public BuildingBehaviour Icon3DPrefab;
    public string Descrpition;
}
