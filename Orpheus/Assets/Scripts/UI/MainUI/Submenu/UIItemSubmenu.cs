using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSubmenu : UISubmenu
{
    [SerializeField] private Button useItemButton;
    
    [SerializeField] private TMP_Text sellButtonText;
    
    private ItemTypes _itemType;
    public void SetItemType(ItemTypes itemType)
    {
        _itemType = itemType;
    }

    public override void Show()
    {
        base.Show();
        useItemButton.interactable = ItemSystem.Instance.CanItemBeUsed(_itemType);
        
        if (sellButtonText != null)
        {
            sellButtonText.SetText($"SELL<sprite=0>{ItemSystem.Instance.GetSellPriceOfItem(_itemType)}");
        }
    }
}
