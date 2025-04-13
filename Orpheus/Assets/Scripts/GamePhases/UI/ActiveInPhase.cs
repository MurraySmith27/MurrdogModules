using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInPhase : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToSetActive;

    [SerializeField] private List<GamePhases> activeInPhases = new();
    
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseTransitionStarted -= OnGamePhaseChanged;
        PhaseStateMachine.Instance.OnPhaseTransitionStarted += OnGamePhaseChanged;
        OnGamePhaseChanged(PhaseStateMachine.Instance.CurrentPhase);
    }
    
    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseTransitionStarted -= OnGamePhaseChanged;
        }
    }

    private void OnEnable()
    {
        OnGamePhaseChanged(PhaseStateMachine.Instance.CurrentPhase);
    }

    private void OnGamePhaseChanged(GamePhases gamePhase)
    {
        gameObjectToSetActive.SetActive(activeInPhases.Contains(gamePhase));
    }
}
