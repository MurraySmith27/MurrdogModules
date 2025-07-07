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
        
        BloomingHarvestController.Instance.OnTileBonusYieldsEnd -= OnTileBonusYieldsComplete;
        BloomingHarvestController.Instance.OnTileBonusYieldsEnd += OnTileBonusYieldsComplete;
        
        BloomingHarvestController.Instance.StartTileBonusYields();
    }

    private void OnTileBonusYieldsComplete()
    {
        _onPhaseEnterComplete?.Invoke();
        BloomingHarvestController.Instance.OnTileBonusYieldsEnd -= OnTileBonusYieldsComplete;
    }
}
