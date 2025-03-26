using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : Singleton<TooltipManager>
{
    [SerializeField] private UITooltip tooltipPrefab;

    [SerializeField] private Transform tooltipParent;

    [SerializeField] private float tooltipTearDownTime = 0.2f;

    private List<UITooltip> _instantiatedTooltips = new();

    private List<int> _tooltipChildren = new(); 

    private List<bool> _tooltipsInUse = new();
    
    public int ShowTooltip(Vector2 tooltipWorldPos, string text)
    {
        int index = CreateTooltip();

        UITooltip newTooltip = _instantiatedTooltips[index];
        
        newTooltip.Populate(tooltipParent.InverseTransformPoint(tooltipWorldPos), text);

        return index;
    }

    public int ShowTooltipChild(Vector2 tooltipWorldPos, string text, int parentIndex)
    {
        if (_tooltipChildren[parentIndex] != -1)
        {
            Debug.LogError("tried to create a child tooltip in a parent that already has a child!");
            return -1;
        }
        
        int childIndex = ShowTooltip(tooltipWorldPos, text);
        
        _tooltipChildren[parentIndex] = childIndex;
        
        return childIndex;
    }

    public bool HideTooltipIfMouseOff(int tooltipIndex)
    {
        if (tooltipIndex >= 0 || tooltipIndex < _instantiatedTooltips.Count)
        {
            if (_tooltipsInUse[tooltipIndex] && !_instantiatedTooltips[tooltipIndex].IsMouseOnTooltip())
            {
                HideTooltip(tooltipIndex);
                return true;
            }
        }

        return false;
    }

    public void HideTooltip(int tooltipIndex)
    {
        if (_tooltipChildren[tooltipIndex] != -1)
        {
            //the child will set it's parent's reference to -1
            HideTooltip(_tooltipChildren[tooltipIndex]);
        }
        
        int indexOfMe = _tooltipChildren.FindIndex((int idx) => idx == tooltipIndex);
        if (indexOfMe != -1)
        {
            _tooltipChildren[indexOfMe] = -1;
        }
        
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
            _tooltipsInUse.Add(true);
            _tooltipChildren.Add(-1);
            return _instantiatedTooltips.Count - 1;
        }
    }
}
