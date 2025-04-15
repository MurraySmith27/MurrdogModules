using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicsList : MonoBehaviour
{

    [SerializeField] private RelicIcon relicIconPrefab;

    [SerializeField] private Transform relicListParent;
    
    [SerializeField] private Vector2Int relicImageDimensions = new Vector2Int(50, 50);
    
    [SerializeField] private Vector2Int relicImageOffset = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int relicImageSize = new Vector2Int(200, 200);

    [SerializeField] private RenderTexture relicImageRenderTexture;
    
    [SerializeField] private RelicVisualsSO relicVisuals;
    
    private Dictionary<RelicTypes, RelicIcon> _instantiatedRelicIcons = new Dictionary<RelicTypes, RelicIcon>();
    
    private Dictionary<RelicTypes, (Preview3DController.PreviewTransform, GameObject)> _instantiatedRelicVisuals = new();
    
    private void Start()
    {
        RelicSystem.Instance.OnRelicAdded -= OnRelicAdded;
        RelicSystem.Instance.OnRelicAdded += OnRelicAdded;
        
        RelicSystem.Instance.OnRelicRemoved -= OnRelicRemoved;
        RelicSystem.Instance.OnRelicRemoved += OnRelicRemoved;
    }

    private void OnDestroy()
    {
        if (RelicSystem.IsAvailable)
        {
            RelicSystem.Instance.OnRelicAdded -= OnRelicAdded;
            RelicSystem.Instance.OnRelicRemoved -= OnRelicRemoved;
        }
    }

    private void OnRelicAdded(RelicTypes relicType)
    {
        if (_instantiatedRelicIcons.ContainsKey(relicType))
        {
            if (_instantiatedRelicIcons[relicType] != null)
            {
                Destroy(_instantiatedRelicIcons[relicType].gameObject);
            }
        }
        
        _instantiatedRelicIcons[relicType] = Instantiate(relicIconPrefab, relicListParent);
        
        CreateRelicVisuals(relicType, _instantiatedRelicIcons[relicType]);
    }
    
    private void CreateRelicVisuals(RelicTypes relicType, RelicIcon relicIcon)
    {
        GameObject prefab = relicVisuals.GetVisualsPrefabForRelic(relicType);

        if (prefab == null)
        {
            Debug.LogError($"RelicVisuals prefab is null. type: {relicType}");
            return;
        }


        if (!Preview3DController.Instance.GetPreviewTransform(
                out Preview3DController.PreviewTransform previewTransform))
        {
            Debug.LogError("No 3d preview transforms ara available. Something went wrong.");
            return;
        }
        
        GameObject instantiatedRelicVisuals = Instantiate(prefab, previewTransform.Transform);
        
        instantiatedRelicVisuals.transform.localPosition = Vector3.zero;

        relicIcon.Populate(previewTransform.UVRect, relicType);
        
        _instantiatedRelicVisuals[relicType] = (previewTransform, instantiatedRelicVisuals);
    }
    
    private void OnRelicRemoved(RelicTypes relicType)
    {
        if (_instantiatedRelicIcons.ContainsKey(relicType))
        {
            if (_instantiatedRelicIcons[relicType] != null)
            {
                Destroy(_instantiatedRelicIcons[relicType].gameObject);
            }
            
            _instantiatedRelicIcons.Remove(relicType);
            
            Destroy(_instantiatedRelicVisuals[relicType].Item2);
            
            Preview3DController.Instance.FreePreviewTransform(_instantiatedRelicVisuals[relicType].Item1);
            
            _instantiatedRelicVisuals.Remove(relicType);
        }
    }
}
