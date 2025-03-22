using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour
{
    [SerializeField]
    private ResourceVisualDataSO resourceVisualData;

    [SerializeField] private bool aimAtCamera = true;
    
    [SerializeField] private List<Image> icons;

    private Camera _mainCamera;

    public void SetIconImage(Sprite sprite)
    {
        foreach (Image icon in icons)
        { 
            icon.sprite = sprite;
        }
    }

    public void SetIconImage(ResourceType type)
    {
        Sprite icon = resourceVisualData.GetSpriteForResourceItem(type);
        
        SetIconImage(icon);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (aimAtCamera) {
            //reorient to look at the camera
            transform.localRotation = Quaternion.Euler(Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up).eulerAngles.x - 90, 0, 0);
        }
    }
}
