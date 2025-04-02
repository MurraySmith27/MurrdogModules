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
    }

    public void GoToNextPhase()
    {
        Debug.LogError("TRY GO NEXT");
        if (!_inPhaseEnterTransition && !_inPhaseExitTransition)
        {
            Debug.LogError("SUCCESS");
            if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingEndStep && _currentHarvestInRound <= numHarvestsBeforeWinter)
            {
                PhaseStateMachine.Instance.ChangePhase(GamePhases.BuddingUpkeep);
            }
            else
            {
                PhaseStateMachine.Instance.ChangePhase((GamePhases)(((int)PhaseStateMachine.Instance.CurrentPhase + 1) %
                                                                Enum.GetNames(typeof(GamePhases)).Length));
            }
            
            _inPhaseEnterTransition = true;
            _inPhaseExitTransition = true;
        }
    }

    private void OnPhaseEnterComplete(GamePhases gamePhase)
    {
        Debug.LogError("PHASE ENTER COMPLETE");
        _inPhaseEnterTransition = false;

        if (!interactablePhases.Contains(gamePhase))
        {
            GoToNextPhase();
        }
    }
    
    private void OnPhaseExitComplete(GamePhases gamePhase)
    {
        Debug.LogError("PHASE EXIT COMPLETE");
        _inPhaseExitTransition = false;
        
        if (!interactablePhases.Contains(gamePhase))
        {
            GoToNextPhase();
        }
    }

    public bool IsInInteractableRound()
    {
        return interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase);
    }
}
