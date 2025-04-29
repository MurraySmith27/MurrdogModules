using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        onEnterComplete?.Invoke();
        
        MapSystem.Instance.GenerateMapChunk(0, 0, GameConstants.STARTING_MAP_SIZE.x, GameConstants.STARTING_MAP_SIZE.y);
        
        GameStartController.Instance.StartGame();
        
        MapSystem.Instance.AddStartingCity();
        
    }
}
