using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemIcon))]
public class ItemTooltip : TooltipBase
{
    [SerializeField] private ItemVisualsSO itemVisuals;
    
    private ItemIcon _itemIcon;

    private void Awake()
    {
        _itemIcon = GetComponent<ItemIcon>();
    }
    
    protected override string GetTooltipText()
    {
        return itemVisuals.GetDescriptionForItem(_itemIcon.GetItemType());
    }
}
