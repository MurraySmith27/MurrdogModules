using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingHarvestYieldBonusesPhase : PhaseStateBase
{
    private Action _onPhaseEnterComplete;
    
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        _onPhaseEnterComplete = onPhaseEnterComplete;
        
        BloomingTileBonusYieldsController.Instance.OnBonusYieldsEnd -= OnTileBonusYieldsComplete;
        BloomingTileBonusYieldsController.Instance.OnBonusYieldsEnd += OnTileBonusYieldsComplete;
        
        BloomingTileBonusYieldsController.Instance.StartTileBonusYields();
    }

    private void OnTileBonusYieldsComplete()
    {
        _onPhaseEnterComplete?.Invoke();
        BloomingTileBonusYieldsController.Instance.OnBonusYieldsEnd -= OnTileBonusYieldsComplete;
    }
}
