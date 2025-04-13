using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingResourceConversionPhase : PhaseStateBase
{
    
    private Action _onPhaseEnterComplete;
    
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        _onPhaseEnterComplete = onPhaseEnterComplete;
        
        BloomingResourceConversionController.Instance.OnResourceConversionEndFinal -= OnHarvestResourceConversionComplete;
        BloomingResourceConversionController.Instance.OnResourceConversionEndFinal += OnHarvestResourceConversionComplete;
        
        BloomingResourceConversionController.Instance.DoResourceConversion();
    }

    private void OnHarvestResourceConversionComplete()
    {
        _onPhaseEnterComplete.Invoke();
        BloomingResourceConversionController.Instance.OnResourceConversionEndFinal -= OnHarvestResourceConversionComplete;
    }
}
