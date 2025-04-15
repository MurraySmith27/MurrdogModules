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

    [Space(10)] 
    [Header("Animation")] 
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorEnterTrigger = "Enter";
    [SerializeField] private string animatorExitTrigger = "Exit";
    [SerializeField] private string animatorOnSelectedTrigger = "Selected";
    [SerializeField] private string animatorOnDeselectedTrigger = "Deselected";
    
    private List<Preview3DController.PreviewTransform> _previewTransforms = new List<Preview3DController.PreviewTransform>();
    
    private TileIcon3DVisual _tile3DVisualInstance;

    private Action _onSelectedCallback;
    
    private void OnDisable()
    {
        Clear();
    }
    
    public void Populate(TileInformation tile, Action onSelectedCallback)
    {
        Clear();

        _onSelectedCallback = onSelectedCallback;
        
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
        
        AnimationUtils.ResetAnimator(animator);
        animator.SetTrigger(animatorEnterTrigger);
    }


    public void Populate(RelicTypes relic, Action onSelectedCallback)
    {
        Clear();
        _onSelectedCallback = onSelectedCallback;
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

    public void ToggleSelected(bool selected)
    {
        if (selected)
        {
            animator.SetTrigger(animatorOnSelectedTrigger);
        }
        else
        {
            animator.SetTrigger(animatorOnDeselectedTrigger);
        }
    }

    public void OnClick()
    {
        _onSelectedCallback?.Invoke();
    }
}
