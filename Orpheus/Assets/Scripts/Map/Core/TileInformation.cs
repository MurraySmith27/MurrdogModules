using System.Collections.Generic;
using UnityEngine.UI;

public enum TileType
{
    None,
    Water,
    Grass,
    Desert,
}

public enum ResourceType
{
    Corn,
    Wheat,
    Fish,
    Wood,
    Stone,
    Bread,
    Lumber,
    Copper,
    Steel,
    Popcorn,
    Sushi,
    Flour,
    Dough,
    Toast,
    ButteredToast,
    Butter,
    Milk,
}

public enum PersistentResourceType
{
    Wood,
    Stone,
    Gold,
    BuildToken,
    Water,
    Dirt,
    Oil
}


public enum BuildingType
{
    CityCapital,
    CornFarm,
    WheatFarm,
    FishFarm,
    Bakery,
    LumberMill,
    CopperYard,
    SteelYard,
    PopcornFactory,
    SushiRestaurant,
    Forest,
    Mine,
    Well,
    DirtPile,
    Mill,
    Toaster,
    CowFarm,
    Stirrer,
    Mixer,
}

public struct TileBuilding
{
    public BuildingType Type;

    public TileBuilding(BuildingType type)
    {
        Type = type;
    }
}

public struct TileInformation
{
    
    public TileType Type;
    
    //resources on a tile
    public List<ResourceItem> Resources;
    
    //buildings on a tile
    public List<TileBuilding> Buildings;

    public TileInformation(TileType tileType)
    {
        Resources = new();
        Buildings = new();
        Type = tileType;
    }
}