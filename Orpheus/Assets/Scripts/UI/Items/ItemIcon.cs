using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    [SerializeField] private TMP_Text sellButtonText;
    
    private ItemTypes _itemType;

    public void Populate(Rect renderTextureUV, ItemTypes itemType)
    {
        rawImage.uvRect = renderTextureUV;
        _itemType = itemType;

        if (sellButtonText != null)
        {
            sellButtonText.SetText($"SELL<sprite=0>{ItemSystem.Instance.GetSellPriceOfItem(_itemType)}");
        }
    }

    public ItemTypes GetItemType()
    {
        return _itemType;
    }

    public void OnItemUseButtonClicked()
    {
        ItemSystem.Instance.UseItem(_itemType);
    }

    public void OnItemSellButtonClick()
    {
        ItemSystem.Instance.SellItem(_itemType);
    }
}
