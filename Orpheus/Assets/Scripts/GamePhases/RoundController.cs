using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundController : Singleton<RoundController>
{
    [SerializeField] private int numHarvestsBeforeWinter = 2;

    private List<GamePhases> interactablePhases = new List<GamePhases>(new GamePhases[]
        { GamePhases.BuddingBuilding, GamePhases.BloomingHarvestTurn });
    
    private int _currentHarvestInRound = 0;

    private bool _inPhaseEnterTransition = true;

    private bool _inPhaseExitTransition = false;

    private bool _inPhaseTransition = true;

    private bool _isInHarvest = false;

    private bool _hasLostGame = false;

    private bool _handUsed = false;
    
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
        
        PhaseStateMachine.Instance.OnPhaseEnterComplete -= OnPhaseEnterComplete;
        PhaseStateMachine.Instance.OnPhaseEnterComplete += OnPhaseEnterComplete;
        
        PhaseStateMachine.Instance.OnPhaseExitComplete -= OnPhaseExitComplete;
        PhaseStateMachine.Instance.OnPhaseExitComplete += OnPhaseExitComplete;

        CitizenController.Instance.OnHandUsed -= OnHandUsed;
        CitizenController.Instance.OnHandUsed += OnHandUsed;

        HarvestState.Instance.OnFoodGoalReached -= OnHarvestGoalReached;
        HarvestState.Instance.OnFoodGoalReached += OnHarvestGoalReached;
        
        HarvestState.Instance.OnHarvestFailed -= OnHarvestFailed;
        HarvestState.Instance.OnHarvestFailed += OnHarvestFailed;

        _currentHarvestInRound = 0;
    }

    private void OnGameStart()
    {
        _hasLostGame = false;
        //assuming we start in budding phase
        _isInHarvest = false;
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
            PhaseStateMachine.Instance.OnPhaseEnterComplete -= OnPhaseEnterComplete;
            PhaseStateMachine.Instance.OnPhaseExitComplete -= OnPhaseExitComplete;
        }

        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnHandUsed -= OnHandUsed;
        }

        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnFoodGoalReached -= OnHarvestGoalReached;
            HarvestState.Instance.OnHarvestFailed -= OnHarvestFailed;
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
        if (!_inPhaseEnterTransition && !_inPhaseExitTransition && !_inPhaseTransition)
        {
            _inPhaseEnterTransition = true;
            _inPhaseExitTransition = true;
            _inPhaseTransition = true;
            
            if (_hasLostGame)
            {
                _hasLostGame = false;
                PhaseStateMachine.Instance.ChangePhase(GamePhases.GameOver);
            }
            else if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingHarvestTurn && _handUsed)
            {
                _handUsed = false;
                _isInHarvest = true;
                PhaseStateMachine.Instance.ChangePhase(GamePhases.BloomingHarvest);
            }
            else if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingHarvest && _isInHarvest)
            {
                _isInHarvest = false;
                PhaseStateMachine.Instance.ChangePhase(GamePhases.BloomingHarvestTurn);
            }
            else if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingEndStep && _currentHarvestInRound <= numHarvestsBeforeWinter)
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
        _inPhaseEnterTransition = false;

        if (!interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase))
        {
            GoToNextPhase();
        }
    }
    
    private void OnPhaseExitComplete(GamePhases gamePhase)
    {
        _inPhaseExitTransition = false;
        
        if (!interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase))
        {
            GoToNextPhase();
        }
    }

    private void OnHandUsed(Dictionary<Guid, List<CitizenController.CitizenPlacement>> citizenPlacements)
    {
        _handUsed = true;
        
        GoToNextPhase();
    }
    
    private void OnHarvestGoalReached()
    {
        _isInHarvest = false;
    }

    private void OnHarvestFailed()
    {
        _isInHarvest = false;
        _hasLostGame = true;
    }

    public bool IsInInteractableRound()
    {
        return interactablePhases.Contains(PhaseStateMachine.Instance.CurrentPhase);
    }

}
