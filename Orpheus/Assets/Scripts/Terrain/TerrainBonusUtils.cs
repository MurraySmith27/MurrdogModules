using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainBonusUtils
{
    public static List<Vector2Int> GetAdjacentTilesWithOuputResourceTag(Vector2Int position, ResourceTags tag)
    {
        List<Vector2Int> adjacentTilesWithOutputResourceTag = new();
        
        List<Vector2Int> adjacentTiles = MapSystem.Instance.GetAdjacentTilesToPosition(position);
        
        adjacentTiles.Add(position);

        foreach (Vector2Int tile in adjacentTiles)
        {
            List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(tile);
            foreach (TileBuilding building in buildingsOnTile)
            {
                List<ResourceItem> outputResources = BuildingsController.Instance.GetAllOutputResourcesOfBuilding(building.Type);

                bool found = false;
                
                foreach (ResourceItem resourceItem in outputResources)
                {
                    if (ResourcesController.Instance.DoesResourceHaveTag(resourceItem.Type, tag))
                    {
                        adjacentTilesWithOutputResourceTag.Add(tile);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }
        }

        return adjacentTilesWithOutputResourceTag;
    }
}
