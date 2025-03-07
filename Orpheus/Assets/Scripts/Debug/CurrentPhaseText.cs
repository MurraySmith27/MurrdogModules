using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentPhaseText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI phaseText;
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(GamePhases newPhase)
    {
        phaseText.SetText($"Current Phase: {Enum.GetName(typeof(GamePhases), newPhase)}");
    }
}
