using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Orpheus/Tile Data", order = 1)]
public class TileDataSO : ScriptableObject
{
    public List<TileDescriptor> tiles;
}
