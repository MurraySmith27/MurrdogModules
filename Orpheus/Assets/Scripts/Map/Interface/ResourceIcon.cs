using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour
{
    [SerializeField] private List<Image> icons;

    private Camera _mainCamera;

    public void SetIconImage(Sprite sprite)
    {
        foreach (Image icon in icons)
        { 
            icon.sprite = sprite;
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        //reorient to look at the camera
        transform.localRotation = Quaternion.Euler(-Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up).eulerAngles.x + 90, 0, 0);
    }
}
