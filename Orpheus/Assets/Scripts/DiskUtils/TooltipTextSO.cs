using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TooltipTextType
{
    None,
    Citizen,
    TileBoosterPack,
    GrassTile,
    WaterTile,
}

[CreateAssetMenu(fileName = "TooltipTextData", menuName = "Orpheus/Tooltip Text Data", order = 1)]
public class TooltipTextSO : ScriptableObject
{
    [Serializable]
    public class TooltipTextData
    {
        public TooltipTextType tooltipType;
        public string text;
    }

    [SerializeField] private List<TooltipTextData> tooltipTextData;

    public string GetTooltipText(TooltipTextType tooltipType)
    {
        TooltipTextData data = tooltipTextData.FirstOrDefault((TooltipTextData data) =>
        {
            return data.tooltipType == tooltipType;
        });

        if (data != null)
        {
            return data.text;
        }
        else
        {
            Debug.LogError($"No tooltip text is defined for type: {Enum.GetName(typeof(TooltipTextType), tooltipType)}");
            return string.Empty;
        }
    }
}
