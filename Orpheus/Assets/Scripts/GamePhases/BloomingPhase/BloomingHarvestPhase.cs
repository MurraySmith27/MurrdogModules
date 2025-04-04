using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingHarvestPhase : PhaseStateBase
{

    private Action _onPhaseEnterComplete;
    
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        _onPhaseEnterComplete = onPhaseEnterComplete;
        
        BloomingHarvestController.Instance.OnHarvestEnd -= OnHarvestComplete;
        BloomingHarvestController.Instance.OnHarvestEnd += OnHarvestComplete;
        
        BloomingHarvestController.Instance.StartHarvest();
    }

    private void OnHarvestComplete()
    {
        BloomingResourceConversionController.Instance.DoResourceConversion();
        
        _onPhaseEnterComplete?.Invoke();
        BloomingHarvestController.Instance.OnHarvestEnd -= OnHarvestComplete;
    }
}
