using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoNextPhaseButton : MonoBehaviour
{
    public void OnClick()
    {
        PhaseStateMachine.Instance.ChangePhase((GamePhases)(((int)PhaseStateMachine.Instance.CurrentPhase + 1) % Enum.GetNames(typeof(GamePhases)).Length));
    }
}
