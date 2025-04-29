using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle3XSpeedButton : MonoBehaviour
{
    public void OnClick()
    {
        if (GlobalSettings.GameSpeed > 1)
        {
            GlobalSettings.GameSpeed = 1;
        }
        else
        {
            GlobalSettings.GameSpeed = 3;
        }
    }
}
