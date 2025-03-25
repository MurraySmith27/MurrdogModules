using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : Singleton<TooltipManager>
{
    [SerializeField] private UITooltip tooltipPrefab;

    [SerializeField] private Transform tooltipParent;

    [SerializeField] private float tooltipTearDownTime = 0.2f;

    private List<UITooltip> _instantiatedTooltips = new();

    private List<bool> _tooltipsInUse = new();
    
    public int ShowTooltip(Vector2 mouseScreenPos, string text)
    {
        int index = CreateTooltip();

        UITooltip newTooltip = _instantiatedTooltips[index];
        
        newTooltip.Populate(mouseScreenPos, text);

        return index;
    }

    public void HideTooltip(int tooltipIndex)
    {
        _instantiatedTooltips[tooltipIndex].TearDown();
        
        AsyncUtils.InvokeCallbackAfterSeconds(tooltipTearDownTime, () =>
        {
            _tooltipsInUse[tooltipIndex] = false;
            _instantiatedTooltips[tooltipIndex].gameObject.SetActive(false);
        });
    }

    private int CreateTooltip()
    {
        int indexOfUnused = _tooltipsInUse.FindIndex((bool x) =>
        {
            return !x;
        });
        
        if (indexOfUnused != -1)
        {
            _tooltipsInUse[indexOfUnused] = true;
            _instantiatedTooltips[indexOfUnused].gameObject.SetActive(true);
            return indexOfUnused;
        }
        else
        {
            _instantiatedTooltips.Add(Instantiate(tooltipPrefab, tooltipParent));
            _tooltipsInUse.Add(false);
            return _instantiatedTooltips.Count - 1;
        }
    }
}
