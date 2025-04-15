using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPackOption : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text cardTypeText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text versionText;

    [SerializeField] private RawImage itemPreviewImage;

    [SerializeField] private TileIcon3DVisual tile3DVisualPrefab;
    
    private List<Preview3DController.PreviewTransform> _previewTransforms = new List<Preview3DController.PreviewTransform>();
    
    private TileIcon3DVisual _tile3DVisualInstance;
    
    private void OnDisable()
    {
        Clear();
    }
    
    public void Populate(TileInformation tile)
    {
        Clear();
        
        titleText.SetText("");
        cardTypeText.SetText($"Tile - {Enum.GetName(typeof(TileType), tile.Type)}");
        descriptionText.SetText(GenerateTileDescription(tile));
        versionText.SetText($"");
        
        if (!Preview3DController.Instance.GetPreviewTransform(out Preview3DController.PreviewTransform previewTransform))
        {
            Debug.LogError("No preview transforms available!");
            return;
        }
        
        _previewTransforms.Add(previewTransform);
        
        _tile3DVisualInstance = Instantiate(tile3DVisualPrefab, previewTransform.Transform);
        
        itemPreviewImage.uvRect = previewTransform.UVRect;
    }


    public void Populate(RelicTypes tile)
    {
        //TODO
    }
    
    private string GenerateTileDescription(TileInformation tile)
    {
        string description = string.Empty;

        if (tile.Buildings.Count > 0)
        {
            description += $"{LocalizationUtils.GetTagForBuilding(tile.Buildings[0].Type)}\n";
        }
        
        foreach (ResourceItem resource in tile.Resources)
        {
            description += $"+{resource.Quantity} {LocalizationUtils.GetTagForResource(resource.Type)}\n";
        }
        
        return description;
    }

    private void Clear()
    {
        foreach (Preview3DController.PreviewTransform previewTransform in _previewTransforms)
        {
            Preview3DController.Instance.FreePreviewTransform(previewTransform);
        }
        
        _previewTransforms.Clear();

        if (_tile3DVisualInstance != null)
        {
            Destroy(_tile3DVisualInstance.gameObject);
        }
    }
}
