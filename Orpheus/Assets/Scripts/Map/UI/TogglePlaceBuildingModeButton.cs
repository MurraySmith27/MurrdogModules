using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TogglePlaceBuildingModeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    
    public void Start()
    {
        buttonText.SetText("Tile Select Mode");
    }
    
    private void SwitchToPlaceBuildingMode()
    {
        MapInteractionController.Instance.SwitchMapInteractionMode(MapInteractionMode.PlaceBuilding);
        buttonText.SetText("Tile Select Mode");
    }

    private void SwitchToDefaultMode()
    {
        MapInteractionController.Instance.SwitchMapInteractionMode(MapInteractionMode.Default);
        buttonText.SetText("Place Building");
    }

    public void OnClick()
    {
        if (MapInteractionController.Instance.CurrentMode == MapInteractionMode.Default)
        {
            SwitchToPlaceBuildingMode();
        }
        else
        {
            SwitchToDefaultMode();
        }
    }
}
