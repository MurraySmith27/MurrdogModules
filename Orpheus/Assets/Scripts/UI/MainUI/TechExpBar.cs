using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechExpBar : MonoBehaviour
{
    [SerializeField] private TMP_Text techExpText;
    [SerializeField] private RectTransform techExpBarFill;
    
    private void Start()
    {
        TechSystem.Instance.OnExpChanged -= UpdateExpBar;
        TechSystem.Instance.OnExpChanged += UpdateExpBar;

        UpdateExpBar();
    }

    private void OnDestroy()
    {
        if (TechSystem.IsAvailable)
        {
            TechSystem.Instance.OnExpChanged -= UpdateExpBar;
        }
    }

    private void UpdateExpBar()
    {
        int currentLevelExp = TechSystem.Instance.GetExpOfCurrentLevel();
        int expToNextLevel = TechSystem.Instance.GetExpUntilNextLevel();

        int totalNextLevelExp = expToNextLevel + currentLevelExp;
        float progress = currentLevelExp / (float)totalNextLevelExp;
        
        techExpBarFill.anchorMax = new Vector2(progress, techExpBarFill.anchorMax.y);
        
        techExpText.SetText($"{currentLevelExp}/{totalNextLevelExp}");
    }
}
