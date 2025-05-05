using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundRewardsOptionsPopup : MonoBehaviour
{

    public void OnSkipButtonClicked()
    {
        Close();
        UIPopupSystem.Instance.HidePopup("RoundRewardsOptionsPopup");
    }

    public void OnAddHexClicked()
    {
        Close();
        BoosterPackSystem.Instance.OpenBoosterPack(BoosterPackTypes.ROUND_REWARDS_TILE_BOOSTER);
    }
    
    public void OnRemoveHexClicked()
    {
        Close();
        MapInteractionController.Instance.SwitchMapInteractionMode(MapInteractionMode.RemoveTile);
    }

    private void Close()
    {
        UIPopupSystem.Instance.HidePopup("RoundRewardsOptionsPopup");
    }
}
