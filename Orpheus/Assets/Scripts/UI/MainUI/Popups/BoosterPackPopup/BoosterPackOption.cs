using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPackOption : MonoBehaviour
{
    [SerializeField] private BuildingsVisualsSO buildingsVisualsSO;
    [SerializeField] private BuildingProcessRulesSO buildingProcessRulesSO;
    
    [SerializeField] private TileIcon3DVisual tile3DVisualPrefab;

    [SerializeField] private OrpheusUIInputChannel inputChannel;
    [SerializeField] private RectTransform hoverOverRectTransform;
    
    [Space(10)]
    
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text cardTypeText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private RawImage itemPreviewImage;

    [Space(10)] 
    [Header("Animation")] 
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorEnterTrigger = "Enter";
    [SerializeField] private string animatorExitTrigger = "Exit";
    [SerializeField] private string animatorOnSelectedTrigger = "Selected";
    [SerializeField] private string animatorOnDeselectedTrigger = "Deselected";
    [SerializeField] private string animatorOnHoveredStartTrigger = "HoverStart";
    [SerializeField] private string animatorOnHoveredEndTrigger = "HoverEnd";
    
    private List<Preview3DController.PreviewTransform> _previewTransforms = new List<Preview3DController.PreviewTransform>();
    
    private TileIcon3DVisual _tile3DVisualInstance;

    private BuildingBehaviour _building3DVisualInstance;

    private Action _onSelectedCallback;

    private bool _isHoveredOver;

    private void Start()
    {
        inputChannel.MouseMoveEvent -= OnMouseMove;
        inputChannel.MouseMoveEvent += OnMouseMove;
    }

    private void OnDestroy()
    {
        if (inputChannel != null)
        {
            inputChannel.MouseMoveEvent -= OnMouseMove;
        }
    }
    
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
        
        _tile3DVisualInstance.Populate(tile);
        
        itemPreviewImage.uvRect = previewTransform.UVRect;
        
        AnimationUtils.ResetAnimator(animator);
        animator.SetTrigger(animatorEnterTrigger);
    }
    
    public void Populate(BuildingType building, Action onSelectedCallback)
    {
        Clear();

        _onSelectedCallback = onSelectedCallback;
        
        titleText.SetText(LocalizationUtils.GetNameOfBuilding(building));
        cardTypeText.SetText($"Building");
        descriptionText.SetText(GenerateTileDescription(building));
        versionText.SetText($"");
        
        if (!Preview3DController.Instance.GetPreviewTransform(out Preview3DController.PreviewTransform previewTransform))
        {
            Debug.LogError("No preview transforms available!");
            return;
        }
        
        _previewTransforms.Add(previewTransform);

        BuildingBehaviour prefab = buildingsVisualsSO.GetIcon3dPrefabForBuilding(building);
        
        _building3DVisualInstance = Instantiate(prefab, previewTransform.Transform);
        
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

    private string GenerateTileDescription(BuildingType building)
    {
        return LocalizationUtils.GetDescriptionOfBuilding(building, buildingsVisualsSO, buildingProcessRulesSO, false);
    }

    private void Clear()
    {
        _isHoveredOver = false;
        
        foreach (Preview3DController.PreviewTransform previewTransform in _previewTransforms)
        {
            Preview3DController.Instance.FreePreviewTransform(previewTransform);
        }
        
        _previewTransforms.Clear();

        if (_tile3DVisualInstance != null)
        {
            Destroy(_tile3DVisualInstance.gameObject);
        }

        if (_building3DVisualInstance != null)
        {
            Destroy(_building3DVisualInstance.gameObject);
        }
    }

    public void ToggleSelected(bool selected)
    {
        ResetAnimatorTriggers();
        if (selected)
        {
            animator.SetTrigger(animatorOnSelectedTrigger);
        }
        else
        {
            animator.SetTrigger(animatorOnDeselectedTrigger);
        }
    }

    private void OnMouseMove(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (RectTransformUtils.IsMouseOverRectTransform(hoverOverRectTransform))
        {
            if (!_isHoveredOver)
            {
                ResetAnimatorTriggers();
                _isHoveredOver = true;
                animator.SetTrigger(animatorOnHoveredStartTrigger);
            }
        }
        else
        {
            if (_isHoveredOver)
            {
                ResetAnimatorTriggers();
                _isHoveredOver = false;
                animator.SetTrigger(animatorOnHoveredEndTrigger);
            }
        }
    }

    private void ResetAnimatorTriggers()
    {
        animator.ResetTrigger(animatorOnHoveredEndTrigger);
        animator.ResetTrigger(animatorOnHoveredStartTrigger);
        animator.ResetTrigger(animatorOnSelectedTrigger);
        animator.ResetTrigger(animatorOnDeselectedTrigger);
    }

    public void OnClick()
    {
        _onSelectedCallback?.Invoke();
    }
}
