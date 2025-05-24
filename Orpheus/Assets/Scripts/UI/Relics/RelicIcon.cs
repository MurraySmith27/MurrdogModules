using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicIcon : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RawImage rawImage;
    
    private RelicTypes _relicType;

    public void Populate(Rect renderTextureUV, RelicTypes relicTypes)
    {
        rawImage.uvRect = renderTextureUV;
        _relicType = relicTypes;
    }

    public RelicTypes GetRelicType()
    {
        return _relicType;
    }

    public void OnTick()
    {
        animator.SetTrigger("Tick");
    }
}
