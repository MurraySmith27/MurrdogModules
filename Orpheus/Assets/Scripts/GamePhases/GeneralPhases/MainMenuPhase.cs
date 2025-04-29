using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        onEnterComplete?.Invoke();
        
        context.ChangePhase(GamePhases.GameStart);
    }
}
