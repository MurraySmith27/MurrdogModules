using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicIcon : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    public void SetRenderTextureUVs(Rect uv)
    {
        rawImage.uvRect = uv;
    }
}
