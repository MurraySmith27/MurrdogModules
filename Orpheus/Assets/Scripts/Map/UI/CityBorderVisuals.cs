using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBorderVisuals : MonoBehaviour
{
    [SerializeField] private Material generatedCityBorderMaterial; 
    
    public void PopulateCityOwnedTiles(List<Vector2Int> ownedTiles)
    {
        //TODO: Generate mesh, and apply the material, make the mesh bounds the outskirts of the border.
    }
}
