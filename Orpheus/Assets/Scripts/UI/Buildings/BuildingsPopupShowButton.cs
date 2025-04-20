using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsPopupShowButton : MonoBehaviour
{
    public void OnClick()
    {
        UIPopupSystem.Instance.ShowPopup("BuildingPopup");
    }
}
