using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInPhase : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToSetActive;

    [SerializeField] private List<GamePhases> activeInPhases = new();
    
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnGamePhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnGamePhaseChanged;
    }
    
    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnGamePhaseChanged;
        }
    }

    private void OnGamePhaseChanged(GamePhases gamePhase)
    {
        gameObjectToSetActive.SetActive(activeInPhases.Contains(gamePhase));
    }
}
