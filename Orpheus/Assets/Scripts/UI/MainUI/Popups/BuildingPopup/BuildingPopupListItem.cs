using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingPopupListItem : MonoBehaviour
{
    [SerializeField] private BuildingsVisualsSO buildingVisuals;
    
    [SerializeField] private BuildingIcon buildingIcon;

    [SerializeField] private TMP_Text buildingNameText;
    
    private BuildingBehaviour _buildingBehaviourInstance;

    private Preview3DController.PreviewTransform _currentPreviewTransform;

    private void OnDisable()
    {
        Clear();
    }
    
    public void Populate(BuildingType buildingType)
    {
        Clear();
        
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
}
