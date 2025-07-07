using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhases
{
    BuddingGoalsUpdate,
    BuddingUpkeep,
    BuddingBuilding,
    BuddingEndStep,
    BloomingUpkeep,
    BloomingHarvestTurn,
    BloomingHarvest,
    BloomingResourceConversion,
    BloomingEndStep,
    WiltingUpkeep,
    WiltingChallenge,
    WiltingExtraResourceConversion,
    WiltingEndStep,
    WiltingRoundUpdatePhase,
    GameWon,
    GameOver,
    MainMenu,
    GameStart,
    BloomingHarvestYieldBonuses,
}

public class PhaseStateMachine : Singleton<PhaseStateMachine>
{
    public GamePhases CurrentPhase
    {
        get
        {
            return currentPhaseState;
        }
    }

    public event Action<GamePhases> OnPhaseTransitionStarted;
    
    public event Action<GamePhases> OnPhaseEnterComplete;
    public event Action<GamePhases> OnPhaseExitComplete;
    
    private PhaseStateBase currentPhase;
    private GamePhases currentPhaseState;

    public Action<GamePhases> OnPhaseChanged;

    private MainMenuPhase _mainMenuPhase = new();
    private GameStartPhase _gameStartPhase = new();
    private BuddingGoalsUpdatePhase _buddingGoalsUpdatePhase = new();
    private BuddingUpkeepPhase _buddingUpkeepPhase = new();
    private BuddingBuildingPhase _buddingBuildingPhase = new();
    private BuddingEndStepPhase _buddingEndStepPhase = new();
    private BloomingUpkeepPhase _bloomingUpkeepPhase = new();
    private BloomingHarvestPhase _bloomingHarvestPhase = new();
    private BloomingHarvestTurnPhase _bloomingHarvestTurnPhase = new();
    private BloomingResourceConversionPhase _bloomingResourceConversionPhase = new();
    private BloomingEndStepPhase _bloomingEndStepPhase = new();
    private WiltingUpkeepPhase _wiltingUpkeepPhase = new();
    private WiltingChallengePhase _wiltingChallengePhase = new();
    private WiltingExtraResourceConversionPhase _wiltingExtraResourceConversionPhase = new();
    private WiltingEndStepPhase _wiltingEndStepPhase = new();
    private WiltingRoundUpdatePhase _wiltingRoundUpdatePhase = new();
    private GameWonPhase _gameWonPhase = new();
    private GameOverPhase _gameOverPhase = new();
    private BloomingHarvestYieldBonusesPhase _bloomingHarvestYieldBonusesPhase = new();

    private Queue<GamePhases> phaseTransitionQueue = new();

    private void Awake()
    {
        currentPhaseState = GamePhases.MainMenu;
        currentPhase = _mainMenuPhase;
    }
    
    private void Start()
    {
        GameStartController.Instance.OnGameStart -= OnGameStart;
        GameStartController.Instance.OnGameStart += OnGameStart;
        
        _mainMenuPhase.StateEnter(this, () =>
        {
            OnPhaseEnterComplete?.Invoke(GamePhases.MainMenu);
            
            RelicSystem.Instance.OnPhaseChanged(GamePhases.MainMenu);
        });
    }

    private void OnDestroy()
    {
        if (GameStartController.IsAvailable)
        {
            GameStartController.Instance.OnGameStart -= OnGameStart;
        }
    }
    
    public void OnGameStart()
    {
        ChangePhase(GamePhases.BuddingUpkeep);
    }

    public void ChangePhase(GamePhases nextPhase)
    {
        bool currentlyTransitioning = phaseTransitionQueue.Count > 0;

        phaseTransitionQueue.Enqueue(nextPhase);

        if (!currentlyTransitioning)
        {
            DoChangePhase();
        }
    }

    private void DoChangePhase()
    {
        GamePhases nextPhase = phaseTransitionQueue.Peek();
        
        OnPhaseTransitionStarted?.Invoke(nextPhase);
        
        currentPhase.StateExit(this, () =>
        {
            OnPhaseExitComplete?.Invoke(currentPhaseState);

            currentPhaseState = nextPhase;

            currentPhase = GetPhaseFromEnumValue(nextPhase);

            currentPhase.StateEnter(this, () =>
            {
                OnPhaseEnterComplete?.Invoke(nextPhase);

                RelicSystem.Instance.OnPhaseChanged(nextPhase);
                
                OnPhaseChanged?.Invoke(nextPhase);
                
                phaseTransitionQueue.Dequeue();

                if (phaseTransitionQueue.Count > 0)
                {
                    DoChangePhase();
                }
            });
        });
    }

    private PhaseStateBase GetPhaseFromEnumValue(GamePhases phase)
    {
        switch (phase)
        {
            case GamePhases.MainMenu:
                return _mainMenuPhase;
            case GamePhases.GameStart:
                return _gameStartPhase;
            case GamePhases.BuddingGoalsUpdate:
                return _buddingGoalsUpdatePhase;
            case GamePhases.BuddingUpkeep:
                return _buddingUpkeepPhase;
            case GamePhases.BuddingBuilding:
                return _buddingBuildingPhase;
            case GamePhases.BuddingEndStep:
                return _buddingEndStepPhase;
            case GamePhases.BloomingUpkeep:
                return _bloomingUpkeepPhase;
            case GamePhases.BloomingHarvestTurn:
                return _bloomingHarvestTurnPhase;
            case GamePhases.BloomingHarvest:
                return _bloomingHarvestPhase;
            case GamePhases.BloomingResourceConversion:
                return _bloomingResourceConversionPhase;
            case GamePhases.BloomingEndStep:
                return _bloomingEndStepPhase;
            case GamePhases.WiltingUpkeep:
                return _wiltingUpkeepPhase;
            case GamePhases.WiltingChallenge:
                return _wiltingChallengePhase;
            case GamePhases.WiltingExtraResourceConversion:
                return _wiltingExtraResourceConversionPhase;
            case GamePhases.WiltingEndStep:
                return _wiltingEndStepPhase;
            case GamePhases.WiltingRoundUpdatePhase:
                return _wiltingRoundUpdatePhase;
            case GamePhases.GameWon:
                return _gameWonPhase;
            case GamePhases.GameOver:
                return _gameOverPhase;
            case GamePhases.BloomingHarvestYieldBonuses:
                return _bloomingHarvestYieldBonusesPhase;
            default:
                return null;
        }
    }
}
