using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingIcon : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    
    private BuildingType _buildingType;

    public void Populate(Rect renderTextureUV, BuildingType buildingType)
    {
        rawImage.uvRect = renderTextureUV;
        _buildingType = buildingType;
    }

    public BuildingType GetBuildingType()
    {
        return _buildingType;
    }
}