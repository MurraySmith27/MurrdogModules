using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIconsOverlayController : MonoBehaviour
{
    [SerializeField] private ResourceIconRetriever resourceIconRetriever;
    [SerializeField] private RectTransform iconsParent;

    [SerializeField] private float iconsDistanceTowardCamera = 1f;
    
    [Space(10)] 
    
    [Header("UI Spacing Options")] 
    [SerializeField] private float iconsSpacing = 5f;
    [SerializeField] private float iconsScale = 1f;

    private List<(ResourceItem, RectTransform)> _instantiatedIcons = new();

    private Vector3 _rootOriginalPosition;
    
    private void Start()
    {
        _rootOriginalPosition = iconsParent.position;
        
        TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnTileCullingUpdated;
        TileFrustrumCulling.Instance.OnTileCullingUpdated += OnTileCullingUpdated;
    }

    private void OnDestroy()
    {
        if (TileFrustrumCulling.IsAvailable)
        {
            TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnTileCullingUpdated;
        }
    }

    private void OnTileCullingUpdated(int row, int col, int width, int height)
    {
        //first clear all instantiated icons:
        foreach ((ResourceItem, RectTransform) icon in _instantiatedIcons)
        {
            resourceIconRetriever.ReturnResourceIcon(icon.Item1, icon.Item2);
        }
        
        for (int i = row; i < row + width; i++)
        {
            for (int j = col; j < col + height; j++)
            {
                SpawnResourceIconsAtTile(i, j);
            }
        }
    }
    
    private void SpawnResourceIconsAtTile(int row, int col)
    {
        Vector2Int gridPosition = new Vector2Int(row, col);
        List<ResourceItem> resources = MapSystem.Instance.GetAllResourcesOnTile(gridPosition);
        
        if (resources.Count == 0) return;
        
        List<RectTransform> iconTransforms = new List<RectTransform>();
                
        foreach (ResourceItem resource in resources)
        {
            RectTransform resourceTransform = resourceIconRetriever.GetResourceIcon(resource);
            resourceTransform.SetParent(iconsParent);
            iconTransforms.Add(resourceTransform);
            _instantiatedIcons.Add((resource, resourceTransform));
        }

        Vector3 tileWorldPosition = MapUtils.GetTileWorldPositionFromGridPosition(gridPosition);

        Vector3 canvasPosition = iconsParent.InverseTransformPoint(tileWorldPosition);

        if (resources.Count == 1)
        {
            iconTransforms[0].localPosition = canvasPosition;
            iconTransforms[0].localScale = new Vector3(iconsScale, iconsScale, iconsScale);
        }
        else if (resources.Count == 2)
        {
            Vector3 spacing = new Vector3(iconsSpacing / 2f, 0, 0);
            iconTransforms[0].localPosition = canvasPosition - spacing;
            iconTransforms[1].localPosition = canvasPosition + spacing;
        }
        else if (resources.Count == 3)
        {
            Vector3 spacing = new Vector3(iconsSpacing, 0, 0);
            iconTransforms[0].localPosition = canvasPosition - spacing;
            iconTransforms[1].localPosition = canvasPosition;
            iconTransforms[2].localPosition = canvasPosition + spacing;
        }
        else if (resources.Count == 4)
        {
            Vector3 spacing = new Vector3(iconsSpacing / 2f, 0, 0);
            iconTransforms[0].localPosition = canvasPosition - spacing * 1.5f;
            iconTransforms[1].localPosition = canvasPosition - spacing;
            iconTransforms[2].localPosition = canvasPosition + spacing;
            iconTransforms[3].localPosition = canvasPosition + spacing * 1.5f;
        }
        else if (resources.Count == 5)
        {
            Vector3 spacing = new Vector3(iconsSpacing, 0, 0);
            iconTransforms[0].localPosition = canvasPosition - spacing * 2f;
            iconTransforms[1].localPosition = canvasPosition - spacing;
            iconTransforms[2].localPosition = canvasPosition;
            iconTransforms[3].localPosition = canvasPosition + spacing;
            iconTransforms[4].localPosition = canvasPosition + spacing * 2f;
        }
        else
        {
            Debug.LogError("Tried to populate more than 5 resource icons on a tile! this is not allowed.");
        }
    }
}
