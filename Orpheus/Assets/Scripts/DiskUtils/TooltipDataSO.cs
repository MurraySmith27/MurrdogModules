using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TooltipIdToText 
{
    public string Id;
    public string Text;
}

[CreateAssetMenu(fileName = "TooltipTextData", menuName = "Orpheus/TooltipTextData", order = 1)]
public class TooltipDataSO : ScriptableObject
{
    
    [SerializeField] private List<TooltipIdToText> tooltipIdToTexts = new List<TooltipIdToText>();
    
    public bool GetTooltipTextFromId(string tooltipId, out string tooltipText)
    {
        tooltipText = "";
        TooltipIdToText tooltipIdToText = tooltipIdToTexts.FirstOrDefault((TooltipIdToText x) =>
        {
            return x.Id == tooltipId;
        });

        if (tooltipIdToText != null)
        {
            tooltipText = tooltipIdToText.Text;
            return true;
        }
        else return false;
    }
}
