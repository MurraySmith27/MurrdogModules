using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicsList : MonoBehaviour
{

    [SerializeField] private RelicIcon relicIconPrefab;

    [SerializeField] private Transform relicListParent;
    
    [SerializeField] private Vector2Int relicImageDimensions = new Vector2Int(50, 50);
    
    [SerializeField] private RenderTexture relicBatchRenderTexture;
    
    [SerializeField] private List<Transform> relicScenePositions;
    
    [SerializeField] private RelicVisualsSO relicVisuals;
    
    private bool[] _occupiedRelicSlots;

    private Dictionary<RelicTypes, RelicIcon> _instantiatedRelicIcons = new Dictionary<RelicTypes, RelicIcon>();
    
    private Dictionary<RelicTypes, (int, GameObject)> _instantiatedRelicVisuals = new();

    private void Awake()
    {
        _occupiedRelicSlots = new bool[relicScenePositions.Count];
    }
    
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
        int unoccupiedSlotIndex = 0;
        for (unoccupiedSlotIndex = 0; unoccupiedSlotIndex < _occupiedRelicSlots.Length; unoccupiedSlotIndex++)
        {
            if (!_occupiedRelicSlots[unoccupiedSlotIndex])
            {
                break;
            }
        }

        if (unoccupiedSlotIndex >= _occupiedRelicSlots.Length)
        {
            Debug.LogWarning("Could not find unoccupied relic slot, this is fine as long as relics are maxed out.");
            return;
        }
        
        GameObject prefab = relicVisuals.GetVisualsPrefabForRelic(relicType);

        if (prefab == null)
        {
            Debug.LogError($"RelicVisuals prefab is null. type: {relicType}");
            return;
        }
        
        GameObject instantiateedRelicVisuals = Instantiate(prefab, relicScenePositions[unoccupiedSlotIndex]);
        
        instantiateedRelicVisuals.transform.localPosition = Vector3.zero;

        int rectMinX = relicImageDimensions.x *
                       (unoccupiedSlotIndex % Mathf.FloorToInt(relicBatchRenderTexture.width / (float)relicImageDimensions.x));
        
        int rectMinY = relicBatchRenderTexture.height - relicImageDimensions.y *
                       Mathf.FloorToInt(unoccupiedSlotIndex / (relicBatchRenderTexture.height / (float)relicImageDimensions.y)) - relicImageDimensions.y;

        relicIcon.Populate(new Rect(
            rectMinX / (float)relicBatchRenderTexture.width,
            rectMinY / (float)relicBatchRenderTexture.height,
            relicImageDimensions.x / (float)relicBatchRenderTexture.width,
            relicImageDimensions.y / (float)relicBatchRenderTexture.height
        ), relicType);
        
        _instantiatedRelicVisuals[relicType] = (unoccupiedSlotIndex, instantiateedRelicVisuals);
        _occupiedRelicSlots[unoccupiedSlotIndex] = true;
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
        
            _occupiedRelicSlots[_instantiatedRelicVisuals[relicType].Item1] = false;
        
            _instantiatedRelicVisuals.Remove(relicType);
        }
    }
}
