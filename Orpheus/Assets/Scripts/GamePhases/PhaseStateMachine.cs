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

public class PhaseStateMachine : MonoBehaviour
{
    private PhaseStateBase currentPhase;

    public BuddingUpkeepPhase BuddingUpkeepPhase;

    void Start()
    {
        currentPhase = BuddingUpkeepPhase;
        
        currentPhase.StateEnter(this);
    }

    public void ChangePhase(GamePhases nextPhase)
    {
        currentPhase.StateExit(this);

        currentPhase = GetPhaseFromEnumValue(nextPhase);

        currentPhase.StateEnter(this);
    }

    private PhaseStateBase GetPhaseFromEnumValue(GamePhases phase)
    {
        //TODO
        return BuddingUpkeepPhase;
    }
}
