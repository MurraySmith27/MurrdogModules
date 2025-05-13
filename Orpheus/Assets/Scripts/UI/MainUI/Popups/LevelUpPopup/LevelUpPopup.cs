using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class LevelUpPopup : MonoBehaviour
{
    [SerializeField] private BuildingsVisualsSO buildingsVisualsSO;
    
    [SerializeField] private TMP_Text rewardText;

    [SerializeField] private BuildingIcon buildingIconPrefab;

    [SerializeField] private GameObject buildingIconsRoot;

    [SerializeField] private Transform buildingIconsParent;
    
    private Dictionary<BuildingType, (Preview3DController.PreviewTransform, BuildingIcon, BuildingBehaviour)> _instantiatedBuildingIcons = new();

    public void Populate(int level)
    {
        Reset();
        
        int numCitizens = TechSystem.Instance.GetRewardOfLevel(level, TechSystemRewardType.CITIZENS);
        int numHands = TechSystem.Instance.GetRewardOfLevel(level, TechSystemRewardType.HANDS);
        int numDiscards = TechSystem.Instance.GetRewardOfLevel(level, TechSystemRewardType.DISCARDS);

        List<string> rewardStrings = new List<string>();
        
        if (numCitizens > 0)
        {
            rewardStrings.Add($"+{numCitizens} Citizens");
        }
        
        if (numHands > 0)
        {
            rewardStrings.Add($"+{numHands} Hands");
        }
        
        if (numDiscards > 0)
        {
            rewardStrings.Add($"+{numDiscards} Discards");
        }
        
        rewardText.text = String.Join("\n", rewardStrings.ToArray());
        
        List<BuildingType> buildingTypes = TechSystem.Instance.GetUnlockedBuildingsOfLevel(level);

        if (buildingTypes.Count > 0)
        {
            buildingIconsRoot.gameObject.SetActive(true);
            foreach (BuildingType type in buildingTypes)
            {
                BuildingIcon buildingIcon = Instantiate(buildingIconPrefab, buildingIconsParent);

                Preview3DController.Instance.GetPreviewTransform(
                    out Preview3DController.PreviewTransform previewTransform);

                buildingIcon.Populate(previewTransform.UVRect, type);
                
                BuildingBehaviour buildingIcon3DPreview = Instantiate(buildingsVisualsSO.GetIcon3dPrefabForBuilding(type), previewTransform.Transform);

                _instantiatedBuildingIcons.Add(type, (previewTransform, buildingIcon, buildingIcon3DPreview));
            }
        }
        else
        {
            buildingIconsRoot.gameObject.SetActive(false);
        }
    }

    private void Reset()
    {
        foreach (KeyValuePair<BuildingType, (Preview3DController.PreviewTransform, BuildingIcon, BuildingBehaviour)> kvp in _instantiatedBuildingIcons)
        {
            Preview3DController.Instance.FreePreviewTransform(kvp.Value.Item1);
            
            Destroy(kvp.Value.Item2.gameObject);
            
            Destroy(kvp.Value.Item3.gameObject);
        }
        
        _instantiatedBuildingIcons.Clear();
    }
    
    public void OnContinueClicked()
    {
        Reset();
        UIPopupSystem.Instance.HidePopup("LevelUpPopup");
    }
}
