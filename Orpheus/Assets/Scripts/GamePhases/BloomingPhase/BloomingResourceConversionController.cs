using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BloomingResourceConversionController
{
    
    public static void ConvertResourcesToFoodScore(Dictionary<ResourceType, int> resourcesToConvert)
    {

        long foodScore = 0;
        
        //TODO
        
        RelicSystem.Instance.OnFoodScoreConversion(foodScore, resourcesToConvert);
    }
}
