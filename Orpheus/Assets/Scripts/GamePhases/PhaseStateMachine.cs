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

    private BuddingGoalsUpdatePhase buddingGoalsUpdatePhase = new();
    private BuddingUpkeepPhase buddingUpkeepPhase = new();
    private BuddingBuildingPhase buddingBuildingPhase = new();
    private BuddingEndStepPhase buddingEndStepPhase = new();
    private BloomingUpkeepPhase bloomingUpkeepPhase = new();
    private BloomingHarvestPhase bloomingHarvestPhase = new();
    private BloomingEndStepPhase bloomingEndStepPhase = new();
    private WiltingUpkeepPhase wiltingUpkeepPhase = new();
    private WiltingChallengePhase wiltingChallengePhase = new();
    private WiltingExtraResourceConversionPhase wiltingExtraResourceConversionPhase = new();
    private WiltingEndStepPhase wiltingEndStepPhase = new();

    void Start()
    {
        currentPhaseState = GamePhases.BuddingUpkeep;
        currentPhase = buddingUpkeepPhase;
        
        currentPhase.StateEnter(this);
        
        OnPhaseChanged?.Invoke(GamePhases.BuddingUpkeep);
    }

    public void ChangePhase(GamePhases nextPhase)
    {
        currentPhase.StateExit(this);

        currentPhaseState = nextPhase;
        
        currentPhase = GetPhaseFromEnumValue(nextPhase);

        currentPhase.StateEnter(this);

        OnPhaseChanged?.Invoke(nextPhase);
    }

    private PhaseStateBase GetPhaseFromEnumValue(GamePhases phase)
    {
        switch (phase)
        {
            case GamePhases.BuddingGoalsUpdate:
                return buddingGoalsUpdatePhase;
            case GamePhases.BuddingUpkeep:
                return buddingUpkeepPhase;
            case GamePhases.BuddingBuilding:
                return buddingBuildingPhase;
            case GamePhases.BuddingEndStep:
                return buddingEndStepPhase;
            case GamePhases.BloomingUpkeep:
                return bloomingUpkeepPhase;
            case GamePhases.BloomingHarvest:
                return bloomingHarvestPhase;
            case GamePhases.BloomingEndStep:
                return bloomingEndStepPhase;
            case GamePhases.WiltingUpkeep:
                return wiltingUpkeepPhase;
            case GamePhases.WiltingChallenge:
                return wiltingChallengePhase;
            case GamePhases.WiltingExtraResourceConversion:
                return wiltingExtraResourceConversionPhase;
            case GamePhases.WiltingEndStep:
                return wiltingEndStepPhase;
            default:
                return null;
        }
    }
}
