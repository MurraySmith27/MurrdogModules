using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGrid
{
    public TileInformation[,] Tiles = new TileInformation[0,0];
    
    public TileInformation this[int col, int row]
    {
        get
        {
            if (col < 0 || col >= Tiles.GetLength(0))
            {
                Debug.LogError($"The provided position ({row}, {col}) is out of bounds of the TileGrid. dimensions: ({Tiles.GetLength(0)}, {Tiles.GetLength(1)}).");
                throw new IndexOutOfRangeException("col");
            }
            else if (row < 0 || row >= Tiles.GetLength(1))
            {
                Debug.LogError($"The provided position ({row}, {col}) is out of bounds of the TileGrid. dimensions: ({Tiles.GetLength(0)}, {Tiles.GetLength(1)}).");
                throw new IndexOutOfRangeException("row");
            }
            else return Tiles[col, row];
        }
        private set
        {
            Tiles[col, row] = value;
        }
    }

    public int GetLength(int dimension)
    {
        return Tiles.GetLength(dimension);
    }

#region BUILDINGS

    public bool HasBuilding(int col, int row, BuildingType buildingType)
    {
        foreach (TileBuilding building in Tiles[col, row].Buildings)
        {
            if (building.Type == buildingType)
            {
                return true;
            }
        }

        return false;
    }

    public List<TileBuilding> GetAllBuildingsOnTile(int col, int row)
    {
        return this[col, row].Buildings.ToList();
    }

    public void AddBuildingToTile(int col, int row, BuildingType buildingType)
    {
        //one building of each type per tile
        if (!HasBuilding(col, row, buildingType))
        {
            TileBuilding newBuilding = new TileBuilding();
            newBuilding.Type = buildingType;
                
            this[col, row].Buildings.Add(newBuilding);
        }
    }
    
    public void RemoveBuildingFromTile(int col, int row, BuildingType buildingType)
    {
        for (int i = 0; i < this[col, row].Buildings.Count; i++)
        {
            if (this[col, row].Buildings[i].Type == buildingType)
            {
                this[col, row].Buildings.RemoveAt(i);
            }
        }
    } 
    
#endregion  
    
#region RESOURCES
    public bool HasResource(int col, int row, ResourceType resourceType, int quantity)
    {
        foreach (ResourceItem resource in Tiles[col, row].Resources)
        {
            if (resource.Type == resourceType && resource.Quantity >= quantity)
            {
                return true;
            }
        }

        return false;
    }

    public List<ResourceItem> GetAllResourcesOnTile(int col, int row)
    {
        return this[col, row].Resources.ToList();
    }

    public void AddResourceToTile(int col, int row, ResourceType resourceType, int quantity)
    {
        bool hadResource = false;
        
        for (int i = 0; i < this[col, row].Resources.Count; i++)
        {
            ResourceItem existingResource = this[col, row].Resources[i]; 
            if (existingResource.Type == resourceType)
            {
                existingResource.Quantity += quantity;
                this[col,row].Resources[i] = existingResource;
                
                hadResource = true;
                break;
            }
        }

        if (!hadResource)
        {
            ResourceItem newResource = new ResourceItem();
            newResource.Type = resourceType;
            newResource.Quantity = quantity;

            this[col, row].Resources.Add(newResource);
        }
    }
    
    public void RemoveResourceFromTile(int col, int row, ResourceType resourceType, int quantity)
    {
        for (int i = 0; i < this[col, row].Resources.Count; i++)
        {
            ResourceItem existingResource = this[col, row].Resources[i]; 
            if (existingResource.Type == resourceType)
            {
                existingResource.Quantity = Mathf.Max(0, existingResource.Quantity - quantity);
                this[col,row].Resources[i] = existingResource;
                return;
            }
        }
    }
    
#endregion
    
    public bool ValidPosition(int col, int row)
    {
        return col < Tiles.GetLength(0) && row < Tiles.GetLength(1) && row >= 0 && col >= 0;
    }

    public void AddChunk(int col, int row, TileInformation[,] chunk)
    {
        int width = chunk.GetLength(0);
        int height = chunk.GetLength(1);

        int existingWidth = GetLength(0);
        int existingHeight = GetLength(1);
        
        if (col + width > existingWidth || row + height > existingHeight)
        {
            AllocateSpace(Mathf.Max(col + width, existingWidth), Mathf.Max(row + height, existingHeight));
        }

        for (int i = col; i < width + col; i++)
        {
            for (int j = row; j < height + row; j++)
            {
                this[i, j] = chunk[i - col, j - row];
            }
        }
    }

    private void AllocateSpace(int requiredWidth, int requiredHeight)
    {
        TileInformation[,] temp = new TileInformation[requiredWidth, requiredHeight];
        for (int i = 0; i < requiredWidth; i++)
        {
            for (int j = 0; j < requiredHeight; j++)
            {
                if (i < Tiles.GetLength(0) && j < Tiles.GetLength(1))
                {
                    temp[i, j] = Tiles[i, j];
                }
                else
                {
                    temp[i, j] = new TileInformation();
                }
            }
        }

        Tiles = temp;
    }
}
