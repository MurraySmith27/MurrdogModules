using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopupListener : MonoBehaviour
{
    private void Start()
    {
        TechSystem.Instance.OnLevelUp -= OnLevelUp;
        TechSystem.Instance.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (TechSystem.IsAvailable)
        {
            TechSystem.Instance.OnLevelUp -= OnLevelUp;
        }
    }

    private void OnLevelUp(int level)
    { 
        // UIPopupComponent popupInstance = UIPopupSystem.Instance.ShowPopup("LevelUpPopup");

        // LevelUpPopup levelUpPopup = popupInstance.GetComponent<LevelUpPopup>();
        
        // levelUpPopup.Populate(level);
    }
}
