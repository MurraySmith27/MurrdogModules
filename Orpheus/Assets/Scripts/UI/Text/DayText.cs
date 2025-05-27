using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayText : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;

    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
        
        SetDayText();
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(GamePhases gamePhases)
    {
        if (gamePhases == GamePhases.BuddingUpkeep) 
        {
            SetDayText();
        }
    }

    private void SetDayText()
    {
        dayText.SetText($"Day {PersistentState.Instance.HarvestNumber + 1}");
    }
}
