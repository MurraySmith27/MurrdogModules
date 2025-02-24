using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour
{
    [SerializeField] private List<Image> icons;

    public void SetIconImage(Sprite sprite)
    {
        foreach (Image icon in icons)
        { 
            icon.sprite = sprite;
        }
    }
}
