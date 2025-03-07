using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TilesVisualsData", menuName = "Orpheus/Tiles Visuals Data", order = 1)]
public class TilesVisualsSO : ScriptableObject
{
    public List<TileVisualsData> TilesVisualsData = new List<TileVisualsData>();
}

[Serializable]
public class TileVisualsData
{
    public TileType Type;
    public TileVisuals Prefab;
}
