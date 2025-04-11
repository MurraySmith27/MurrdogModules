using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCitizenItem : Item
{
    public override bool OnItemUsed(out AdditionalTriggeredArgs args)
    {
        args = new();
        
        if (PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingHarvestTurn)
        {
            List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

            if (cityGuids.Count > 0)
            {
                bool success = CitizenController.Instance.TryPlaceBonusCitizenOnRandomTile(cityGuids[0]);

                if (success)
                {
                    args.IntArg = 1;
                    return true;
                }
            }
        }
        
        return false;
    }

    public override bool IsItemUsable()
    {
        return PhaseStateMachine.Instance.CurrentPhase == GamePhases.BloomingHarvestTurn && MapSystem.Instance.GetAllCityGuids().Count > 0;
    } 

    public override void SerializeItem()
    {
        
    }
}
