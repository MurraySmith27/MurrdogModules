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
    BloomingHarvest,
    BloomingResourceConversion,
    BloomingEndStep,
    WiltingUpkeep,
    WiltingChallenge,
    WiltingExtraResourceConversion,
    WiltingEndStep,
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
    
    private PhaseStateBase currentPhase;
    private GamePhases currentPhaseState;

    public Action<GamePhases> OnPhaseChanged;

    private BuddingGoalsUpdatePhase _buddingGoalsUpdatePhase = new();
    private BuddingUpkeepPhase _buddingUpkeepPhase = new();
    private BuddingBuildingPhase _buddingBuildingPhase = new();
    private BuddingEndStepPhase _buddingEndStepPhase = new();
    private BloomingUpkeepPhase _bloomingUpkeepPhase = new();
    private BloomingHarvestPhase _bloomingHarvestPhase = new();
    private BloomingResourceConversionPhase _bloomingResourceConversionPhase = new();
    private BloomingEndStepPhase _bloomingEndStepPhase = new();
    private WiltingUpkeepPhase _wiltingUpkeepPhase = new();
    private WiltingChallengePhase _wiltingChallengePhase = new();
    private WiltingExtraResourceConversionPhase _wiltingExtraResourceConversionPhase = new();
    private WiltingEndStepPhase _wiltingEndStepPhase = new();

    void Start()
    {
        currentPhaseState = GamePhases.BuddingUpkeep;
        currentPhase = _buddingUpkeepPhase;
        
        currentPhase.StateEnter(this);
        
        RelicSystem.Instance.OnPhaseChanged(GamePhases.BuddingUpkeep);
        
        OnPhaseChanged?.Invoke(GamePhases.BuddingUpkeep);
    }

    public void ChangePhase(GamePhases nextPhase)
    {
        currentPhase.StateExit(this);

        currentPhaseState = nextPhase;
        
        currentPhase = GetPhaseFromEnumValue(nextPhase);

        currentPhase.StateEnter(this);
        
        RelicSystem.Instance.OnPhaseChanged(nextPhase);

        OnPhaseChanged?.Invoke(nextPhase);
    }

    private PhaseStateBase GetPhaseFromEnumValue(GamePhases phase)
    {
        switch (phase)
        {
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
            default:
                return null;
        }
    }
}
