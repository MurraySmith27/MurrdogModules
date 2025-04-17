using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationUtils
{


    public static string GetNameOfResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return "Wood";
            case ResourceType.Stone:
                return "Stone";
            case ResourceType.Corn:
                return "Corn";
            case ResourceType.Wheat:
                return "Wheat";
            case ResourceType.Fish:
                return "Fish";
            case ResourceType.Bread:
                return "Bread";
            default:
                return "";
        }
    }

    public static string GetTagForResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return "<wood>";
            case ResourceType.Stone:
                return "<stone>";
            case ResourceType.Corn:
                return "<corn>";
            case ResourceType.Wheat:
                return "<wheat>";
            case ResourceType.Fish:
                return "<fish>";
            case ResourceType.Bread:
                return "<bread>";
            default:
                return "";
        }
    }

    public static string GetTagForBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.CityCapital:
                return "<citycapital>";
            case BuildingType.CornFarm:
                return "<cornfarm>";
            case BuildingType.FishFarm:
                return "<fishfarm>";
            case BuildingType.WheatFarm:
                return "<wheatfarm>";
            case BuildingType.Bakery:
                return "<bakery>";
            default:
                return "";
        }
    }
}
