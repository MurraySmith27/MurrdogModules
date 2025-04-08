using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    
    private ItemTypes _itemType;

    public void Populate(Rect renderTextureUV, RelicTypes itemType)
    {
        rawImage.uvRect = renderTextureUV;
        _itemType = itemType;
    }

    public ItemTypes GetItemType()
    {
        return _itemType;
    }
}
