using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPackIcon : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    
    private BoosterPackTypes _boosterPackType;

    public void Populate(Rect renderTextureUV, BoosterPackTypes boosterPackType)
    {
        rawImage.uvRect = renderTextureUV;
        _boosterPackType = boosterPackType;
    }

    public BoosterPackTypes GetBoosterPackType()
    {
        return _boosterPackType;
    }
}
