using System.Collections.Generic;

public enum TileType
{
    None,
    Water,
    Grass
}

public enum ResourceType
{
    Corn,
    Wheat,
    Fish,
    Wood,
    Stone
}

public enum PersistentResourceType
{
    Wood,
    Stone,
    Gold
}


public enum BuildingType
{
    CornFarm,
    WheatFarm,
    FishFarm,
    Bakery,
    CityCapital
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