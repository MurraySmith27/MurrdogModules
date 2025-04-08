using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemsList : MonoBehaviour
{

    [SerializeField] private ItemIcon itemIconPrefab;

    [SerializeField] private Transform itemListParent;
    
    [SerializeField] private Vector2Int itemImageDimensions = new Vector2Int(50, 50);
    
    [SerializeField] private RenderTexture itemBatchRenderTexture;
    
    [SerializeField] private List<Transform> itemScenePositions;
    
    [SerializeField] private ItemVisualsSO itemVisuals;
    
    private bool[] _occupiedItemSlots;

    private Dictionary<ItemTypes, ItemIcon> _instantiatedItemIcons = new Dictionary<ItemTypes, ItemIcon>();
    
    private Dictionary<ItemTypes, (int, GameObject)> _instantiatedItemVisuals = new();

    private void Awake()
    {
        _occupiedItemSlots = new bool[itemScenePositions.Count];
    }
    
    private void Start()
    {
        ItemSystem.Instance.OnItemAdded -= OnItemAdded;
        ItemSystem.Instance.OnItemAdded += OnItemAdded;
        
        ItemSystem.Instance.OnItemRemoved -= OnItemRemoved;
        ItemSystem.Instance.OnItemRemoved += OnItemRemoved;
    }

    private void OnDestroy()
    {
        if (ItemSystem.IsAvailable)
        {
            ItemSystem.Instance.OnItemAdded -= OnItemAdded;
            ItemSystem.Instance.OnItemRemoved -= OnItemRemoved;
        }
    }

    private void OnItemAdded(ItemTypes itemType)
    {
        if (_instantiatedItemIcons.ContainsKey(itemType))
        {
            if (_instantiatedItemIcons[itemType] != null)
            {
                Destroy(_instantiatedItemIcons[itemType].gameObject);
            }
        }
        
        _instantiatedItemIcons[itemType] = Instantiate(itemIconPrefab, itemListParent);
        
        CreateItemVisuals(itemType, _instantiatedItemIcons[itemType]);
    }
    
    private void CreateItemVisuals(ItemTypes itemType, ItemIcon itemIcon)
    {
        int unoccupiedSlotIndex = 0;
        for (unoccupiedSlotIndex = 0; unoccupiedSlotIndex < _occupiedItemSlots.Length; unoccupiedSlotIndex++)
        {
            if (!_occupiedItemSlots[unoccupiedSlotIndex])
            {
                break;
            }
        }

        if (unoccupiedSlotIndex >= _occupiedItemSlots.Length)
        {
            Debug.LogWarning("Could not find unoccupied item slot, this is fine as long as items are maxed out.");
            return;
        }
        
        GameObject prefab = itemVisuals.GetVisualsPrefabForItem(itemType);

        if (prefab == null)
        {
            Debug.LogError($"ItemVisuals prefab is null. type: {itemType}");
            return;
        }
        
        GameObject instantiateedItemVisuals = Instantiate(prefab, itemScenePositions[unoccupiedSlotIndex]);
        
        instantiateedItemVisuals.transform.localPosition = Vector3.zero;

        int rectMinX = itemImageDimensions.x *
                       (unoccupiedSlotIndex % Mathf.FloorToInt(itemBatchRenderTexture.width / (float)itemImageDimensions.x));
        
        int rectMinY = itemBatchRenderTexture.height - itemImageDimensions.y *
                       Mathf.FloorToInt(unoccupiedSlotIndex / (itemBatchRenderTexture.height / (float)itemImageDimensions.y)) - itemImageDimensions.y;

        itemIcon.Populate(new Rect(
            rectMinX / (float)itemBatchRenderTexture.width,
            rectMinY / (float)itemBatchRenderTexture.height,
            itemImageDimensions.x / (float)itemBatchRenderTexture.width,
            itemImageDimensions.y / (float)itemBatchRenderTexture.height
        ), itemType);
        
        _instantiatedItemVisuals[itemType] = (unoccupiedSlotIndex, instantiateedItemVisuals);
        _occupiedItemSlots[unoccupiedSlotIndex] = true;
    }
    
    private void OnItemRemoved(ItemTypes itemType)
    {
        if (_instantiatedItemIcons.ContainsKey(itemType))
        {
            if (_instantiatedItemIcons[itemType] != null)
            {
                Destroy(_instantiatedItemIcons[itemType].gameObject);
            }
            
            _instantiatedItemIcons.Remove(itemType);
            
            Destroy(_instantiatedItemVisuals[itemType].Item2);
        
            _occupiedItemSlots[_instantiatedItemVisuals[itemType].Item1] = false;
        
            _instantiatedItemVisuals.Remove(itemType);
        }
    }
}