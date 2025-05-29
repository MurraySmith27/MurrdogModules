using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPackPopupListener : MonoBehaviour
{
    [SerializeField] private BoosterPackOpeningPopup popup;
    
    private void Start()
    {
        BoosterPackSystem.Instance.OnBoosterPackOpened -= OnBoosterPackOpened;
        BoosterPackSystem.Instance.OnBoosterPackOpened += OnBoosterPackOpened;
    }

    private void OnDestroy()
    {
        if (BoosterPackSystem.IsAvailable)
        {
            BoosterPackSystem.Instance.OnBoosterPackOpened -= OnBoosterPackOpened;
        }
    }

    private void OnBoosterPackOpened(BoosterPackTypes type)
    {
        UIPopupSystem.Instance.ShowPopup("BoosterPackOpeningPopup");//, PanelShowBehaviour.HIDE_PREVIOUS);

        popup.PopulateBoosterPackContents();
    }
}
