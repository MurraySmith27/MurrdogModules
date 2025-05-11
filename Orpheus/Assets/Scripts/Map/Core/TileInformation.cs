using System.Collections.Generic;

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
}

public struct TileBuilding
{
    public BuildingType Type;
}

public struct TileInformation
{
    
    public TileType Type;
    
    //resources on a tile
    public List<ResourceItem> Resources;
    
    //buildings on a tile
    public List<TileBuilding> Buildings;
}