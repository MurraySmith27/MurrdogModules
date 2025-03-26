using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : Singleton<TooltipManager>
{
    [SerializeField] private UITooltip tooltipPrefab;

    [SerializeField] private Transform tooltipParent;

    [SerializeField] private float tooltipTearDownTime = 0.2f;
    
    [SerializeField] private TooltipDataSO tooltipDataSO;

    private List<UITooltip> _instantiatedTooltips = new();

    private List<int> _tooltipChildren = new(); 

    private List<bool> _tooltipsInUse = new();

    private List<string> _tooltipTexts = new();
    
    private List<Action> _tooltipHideCallbacks = new();

    public int ShowTooltipById(Vector2 tooltipWorldPosition, string tooltipId, Action onTooltipHide = null, bool isChild = false)
    {
        string tooltipText;
        if (tooltipDataSO.GetTooltipTextFromId(tooltipId, out tooltipText))
        {
            return ShowTooltip(tooltipWorldPosition, tooltipText, onTooltipHide, isChild);
        }
        else
        {
            Debug.LogError("Can't find tooltip with id: " + tooltipId);
            return -1;
        }
    }
    
    public int ShowTooltip(Vector2 tooltipWorldPos, string text, Action onTooltipHide = null, bool isChild = false)
    {
        if (_tooltipTexts.Contains(text))
        {
            return -1;
        }
        
        int index = CreateTooltip(text, onTooltipHide);
        
        if (!isChild)
        {
            //hide existing non-child tooltips
            for (int i = 0; i < _tooltipsInUse.Count; i++)
            {
                if (i != index && _tooltipsInUse[i] && !_tooltipChildren.Contains(i))
                {
                    HideTooltip(i);
                }
            }
        }

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
        
        int childIndex = ShowTooltip(tooltipWorldPos, text, isChild: true);
        
        _tooltipChildren[parentIndex] = childIndex;
        
        return childIndex;
    }

    public bool HideTooltipIfMouseOff(int tooltipIndex)
    {
        if (tooltipIndex >= 0 && tooltipIndex < _instantiatedTooltips.Count)
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
            _tooltipTexts[tooltipIndex] = string.Empty;
            _tooltipHideCallbacks[tooltipIndex]?.Invoke();
            _tooltipHideCallbacks[tooltipIndex] = null;
            _instantiatedTooltips[tooltipIndex].gameObject.SetActive(false);
        });
    }

    private int CreateTooltip(string text, Action onTooltipHide)
    {
        int indexOfUnused = _tooltipsInUse.FindIndex((bool x) =>
        {
            return !x;
        });
        
        if (indexOfUnused != -1)
        {
            _tooltipsInUse[indexOfUnused] = true;
            _tooltipTexts[indexOfUnused] = text;
            _instantiatedTooltips[indexOfUnused].gameObject.SetActive(true);
            _tooltipHideCallbacks[indexOfUnused] = onTooltipHide;
            return indexOfUnused;
        }
        else
        {
            _instantiatedTooltips.Add(Instantiate(tooltipPrefab, tooltipParent));
            _tooltipsInUse.Add(true);
            _tooltipTexts.Add(text);
            _tooltipChildren.Add(-1);
            _tooltipHideCallbacks.Add(onTooltipHide);
            return _instantiatedTooltips.Count - 1;
        }
    }
}
