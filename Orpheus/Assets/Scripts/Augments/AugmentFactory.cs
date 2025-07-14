using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentFactory 
{
    public Augment CreateAugment(AugmentTypes augmentTypes)
    {
        switch (augmentTypes)
        {
            case AugmentTypes.FOIL:
                return new FoilAugment();
            case AugmentTypes.HOLO:
                return new HoloAugment();
            case AugmentTypes.GOLDEN:
                return new GoldenAugment();
            default:
                return null;
        }
    }
}
