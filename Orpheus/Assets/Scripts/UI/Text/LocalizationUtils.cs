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
            case ResourceType.Lumber:
                return "Lumber";
            case ResourceType.Copper:
                return "Copper";
            case ResourceType.Steel:
                return "Steel";
            case ResourceType.Popcorn:
                return "Popcorn";
            case ResourceType.Sushi:
                return "Sushi";
            default:
                return "";
        }
    }

    public static string GetNameOfBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.CityCapital:
                return "Castle";
            case BuildingType.CornFarm:
                return "Corn Farm";
            case BuildingType.WheatFarm:
                return "Wheat Farm";
            case BuildingType.Bakery:
                return "Bakery";
            case BuildingType.FishFarm:
                return "Fish Farm";
            case BuildingType.LumberMill:
                return "Lumber Mill";
            case BuildingType.CopperYard:
                return "Copper Yard";
            case BuildingType.SteelYard:
                return "Steel Factory";
            case BuildingType.PopcornFactory:
                return "Popcorn Factory";
            case BuildingType.SushiRestaurant:
                return "Sushi Restaurant";
            default:
                return "";
        }
    }

    public static string GetTagForPersistentResource(PersistentResourceType persistentResourceType)
    {
        switch (persistentResourceType)
        {
            case PersistentResourceType.Wood:
                return "<wood>";
            case PersistentResourceType.Stone:
                return "<stone>";
            case PersistentResourceType.Gold:
                return "<gold>";
            default:
                return "";
        }
    }
    
    public static string GetIconTagForPersistentResource(PersistentResourceType persistentResourceType)
    {
        switch (persistentResourceType)
        {
            case PersistentResourceType.Wood:
                return "<sprite index=1>";
            case PersistentResourceType.Stone:
                return "<sprite index=3>";
            case PersistentResourceType.Gold:
                return "<sprite index=0>";
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
            case ResourceType.Lumber:
                return "<lumber>";
            case ResourceType.Copper:
                return "<copper>";
            case ResourceType.Steel:
                return "<steel>";
            case ResourceType.Popcorn:
                return "<popcorn>";
            case ResourceType.Sushi:
                return "<sushi>";
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
            case BuildingType.LumberMill:
                return "<lumbermill>";
            case BuildingType.CopperYard:
                return "<copperyard>";
            case BuildingType.SteelYard:
                return "<steelyard>";
            case BuildingType.PopcornFactory:
                return "<popcornfactory>";
            case BuildingType.SushiRestaurant:
                return "<sushirestaurant>";
            default:
                return "";
        }
    }
}
