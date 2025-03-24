using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CityTileData
{
    public readonly Guid CityGuid;
    
    private List<Vector2Int> _ownedTiles = new List<Vector2Int>();

    private Vector2Int _capitalLocation;

    public CityTileData(Vector2Int capitalLocation, List<Vector2Int> ownedTiles)
    {
        this.CityGuid = Guid.NewGuid();
        this._capitalLocation = capitalLocation;
        this._ownedTiles = ownedTiles;
        
        _ownedTiles.Sort(((Vector2Int a, Vector2Int b) =>
        {
            //sort high y values near the start, then sort by low x values near the start
            int compare = -a.x.CompareTo(b.x);

            if (compare == 0)
            {
                return -a.y.CompareTo(b.y);
            }
            else return compare;
        }));
    }
    
    public bool IsLocationInCity(Vector2Int tileLocation)
    {
        foreach (Vector2Int ownedTile in this._ownedTiles)
        {
            if (ownedTile == tileLocation) return true;
        }

        return false;
    }

    public Vector2Int GetCapitalLocation()
    {
        return _capitalLocation;
    }

    public void AddTileToCity(Vector2Int newTile)
    {
        if (!IsLocationInCity(newTile)) 
        {
            //keep the list sorted by high y values first, then low x values first
            for (int i = 0; i < _ownedTiles.Count; i++)
            {
                if (_ownedTiles[i].y < newTile.y)
                {
                    _ownedTiles.Insert(i, newTile);
                    return;
                }
                else if (_ownedTiles[i].y == newTile.y)
                {
                    if (_ownedTiles[i].x >= newTile.x)
                    {
                        _ownedTiles.Insert(i, newTile);
                        return;
                    }
                }
            }
            
            _ownedTiles.Add(newTile);
        }
    }

    public void AddTilesToCity(List<Vector2Int> newTiles)
    {
        foreach (Vector2Int newTile in newTiles) 
        {
            AddTileToCity(newTile);    
        }
    }

    public List<Vector2Int> GetTilesInOrder()
    {
        return _ownedTiles; //they're already sorted
    }
}
