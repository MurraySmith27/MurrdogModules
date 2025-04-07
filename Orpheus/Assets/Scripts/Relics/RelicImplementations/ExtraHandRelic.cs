using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHandRelic : Relic
{
    public override bool OnHarvestStart(out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        
        HarvestState.Instance.AddExtraHands(1);
        return true;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
