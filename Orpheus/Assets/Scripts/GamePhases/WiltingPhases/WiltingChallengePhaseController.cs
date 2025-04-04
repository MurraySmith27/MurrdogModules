using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingChallengePhaseController : Singleton<WiltingChallengePhaseController>
{
    public event Action OnWiltingChallengePassed;
    
    public event Action OnWiltingChallengeFailed;
    
    public void StartChallengePhase()
    {
        
    }
}
