using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingResourceConversionPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        BloomingResourceConversionController.Instance.DoResourceConversion();

        onPhaseEnterComplete?.Invoke();
    }
}
