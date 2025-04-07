using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraDiscardRelic : Relic
{
    public override bool OnHarvestStart(out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        
        HarvestState.Instance.AddExtraDiscards(1);
        return true;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
