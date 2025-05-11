using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        HarvestState.Instance.OnHarvestStart += OnHarvestStart;

        HarvestState.Instance.OnCurrentFoodScoreChanged -= SetHarvestText;
        HarvestState.Instance.OnCurrentFoodScoreChanged += SetHarvestText;
        
        SetHarvestText(HarvestState.Instance.CurrentFoodScore);
    }

    private void OnDestroy()
    {
        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
            HarvestState.Instance.OnCurrentFoodScoreChanged -= SetHarvestText;
        }
    }

    private void OnHarvestStart()
    {
        SetHarvestText(HarvestState.Instance.CurrentFoodScore);
    }

    private void SetHarvestText(long foodScore)
    {
        scoreText.SetText($"<sprite index=11><bounce a=0.1 f=0.5>{HarvestState.Instance.CurrentFoodScore}</bounce>");
    }
}
