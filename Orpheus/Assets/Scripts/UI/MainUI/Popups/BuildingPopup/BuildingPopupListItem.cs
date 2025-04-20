using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class BuildingPopupListItem : MonoBehaviour
{
    [SerializeField] private BuildingsVisualsSO buildingVisuals;
    
    [SerializeField] private BuildingIcon buildingIcon;

    [SerializeField] private TMP_Text buildingNameText;

    [SerializeField] private TMP_Text buildingCostText;
    
    private BuildingBehaviour _buildingBehaviourInstance;

    private Preview3DController.PreviewTransform _currentPreviewTransform;

    private BuildingType _currentBuildingType;

    private void OnDisable()
    {
        Clear();
    }
    
    public void Populate(BuildingType buildingType)
    {
        Clear();

        _currentBuildingType = buildingType;
        
        BuildingVisualsData visualData = buildingVisuals.BuildingsVisualsData.FirstOrDefault((BuildingVisualsData data) =>
        {
            return data.Type == buildingType;
        });

        if (visualData == null)
        {
            Debug.LogError($"No such building visuals data exists for building type {Enum.GetName(typeof(BuildingType), buildingType)}");
            return;
        }

        Preview3DController.PreviewTransform previewTransform;
        if (!Preview3DController.Instance.GetPreviewTransform(out previewTransform))
        {
            Debug.LogError("Tried to request a 3d preview transform, but none are available!");
            return;
        }

        List<PersistentResourceItem> costs = BuildingsController.Instance.GetBuildingCost(_currentBuildingType);
        
        StringBuilder costText = new();
        
        for (int i = 0; i < costs.Count; i++)
        {
            costText.Append($"{LocalizationUtils.GetIconTagForPersistentResource(costs[i].Type)}{costs[i].Quantity}");

            if (i != costs.Count - 1)
            {
                costText.Append(" ");
            }
        }
        
        buildingCostText.SetText(costText.ToString());
        
        _currentPreviewTransform = previewTransform;

        _buildingBehaviourInstance = Instantiate(visualData.Icon3DPrefab, previewTransform.Transform);

        _buildingBehaviourInstance.Populate();
        
        buildingIcon.Populate(previewTransform.UVRect, buildingType);
        
        buildingNameText.SetText(LocalizationUtils.GetNameOfBuilding(buildingType));
    }

    private void Clear()
    {
        if (_currentPreviewTransform != null)
        {
            Preview3DController.Instance.FreePreviewTransform(_currentPreviewTransform);
            _currentPreviewTransform = null;
        }
        
        if (_buildingBehaviourInstance != null)
            Destroy(_buildingBehaviourInstance.gameObject);
    }

    public void OnClick()
    {
        if (BuildingsController.Instance.HasBuildingCost(_currentBuildingType))
        {
            MapInteractionController.Instance.SwitchToPlaceBuildingMode(_currentBuildingType);
        }
    }
}
