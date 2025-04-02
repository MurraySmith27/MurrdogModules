using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundController : Singleton<RoundController>
{
    [SerializeField] private int numHarvestsBeforeWinter = 2;

    public List<GamePhases> interactablePhases = new List<GamePhases>(new GamePhases[]
        { GamePhases.BuddingBuilding, GamePhases.BloomingEndStep, GamePhases.WiltingEndStep });
    
    private int _currentHarvestInRound = 0;

    private bool _inPhaseEnterTransition = true;

    private bool _inPhaseExitTransition = false;

    private bool _inPhaseTransition = true;
    
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
        
        PhaseStateMachine.Instance.OnPhaseEnterComplete -= OnPhaseEnterComplete;
        PhaseStateMachine.Instance.OnPhaseEnterComplete += OnPhaseEnterComplete;
        
        PhaseStateMachine.Instance.OnPhaseExitComplete -= OnPhaseExitComplete;
        PhaseStateMachine.Instance.OnPhaseExitComplete += OnPhaseExitComplete;

        _currentHarvestInRound = 0;
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
            PhaseStateMachine.Instance.OnPhaseEnterComplete -= OnPhaseEnterComplete;
            PhaseStateMachine.Instance.OnPhaseExitComplete -= OnPhaseExitComplete;
        }
    }

    private void OnPhaseChanged(GamePhases gamePhase)
    {
        if (gamePhase == GamePhases.BuddingUpkeep)
        {
            _currentHarvestInRound++;
        }
        else if (gamePhase == GamePhases.WiltingEndStep)
        {
            _currentHarvestInRound = 0;
        }

        _inPhaseTransition = false;
        
        if (!interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase))
        {
            GoToNextPhase();
        }
    }

    public void GoToNextPhase()
    {
        Debug.LogError("GoToNextPhase Called");
        if (!_inPhaseEnterTransition && !_inPhaseExitTransition && !_inPhaseTransition)
        {
            Debug.LogError($"GoToNextPhase, current: {PhaseStateMachine.Instance.CurrentPhase}");
            _inPhaseEnterTransition = true;
            _inPhaseExitTransition = true;
            _inPhaseTransition = true;
            if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingEndStep && _currentHarvestInRound <= numHarvestsBeforeWinter)
            {
                PhaseStateMachine.Instance.ChangePhase(GamePhases.BuddingUpkeep);
            }
            else
            {
                PhaseStateMachine.Instance.ChangePhase((GamePhases)(((int)PhaseStateMachine.Instance.CurrentPhase + 1) %
                                                                Enum.GetNames(typeof(GamePhases)).Length));
            }
        }
    }

    private void OnPhaseEnterComplete(GamePhases gamePhase)
    {
        Debug.LogError("on phase enter complete");
        _inPhaseEnterTransition = false;

        if (!interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase))
        {
            GoToNextPhase();
        }
    }
    
    private void OnPhaseExitComplete(GamePhases gamePhase)
    {
        
        Debug.LogError("On phase exit complete");
        _inPhaseExitTransition = false;
        
        if (!interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase))
        {
            GoToNextPhase();
        }
    }

    public bool IsInInteractableRound()
    {
        return interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase);
    }
}
