using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

public class TooltipManager : Singleton<TooltipManager>
{
    private class TooltipState
    {
        public UITooltip instantiatedTooltip;
        public int child;
        public bool inUse;
        public string currentText;
        public Action onHideCallback;
        public bool isHiding;
        public CoroutineHandle tooltipAttemptHideCoroutine;
    }
    
    [SerializeField] private UITooltip tooltipPrefab;

    [SerializeField] private Transform tooltipParent;

    [SerializeField] private float tooltipTearDownTime = 0.2f;

    [SerializeField] private float tooltipMouseOffHideTime = 0.2f;
    
    [SerializeField] private TooltipDataSO tooltipDataSO;

    [SerializeField] private bool spawnChildrenOutsideOfParent = true;

    private List<TooltipState> _tooltips = new List<TooltipState>();

    private int _lastRootTooltipIndexOpened = -1;

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

    public int ShowTooltipChildByTooltipId(Vector2 tooltipWorldPosition, string tooltipId, Action onTooltipHide = null,
        bool isChild = false)
    {
        string tooltipText;
        if (tooltipDataSO.GetTooltipTextFromId(tooltipId, out tooltipText))
        {
            return ShowTooltipChild(tooltipWorldPosition, tooltipText, onTooltipHide);
        }
        else
        {
            Debug.LogError("Can't find tooltip with id: " + tooltipId);
            return -1;
        }
    }
    
    public int ShowTooltip(Vector2 tooltipWorldPos, string text, Action onTooltipHide = null, bool isChild = false)
    {
        if (_tooltips.FirstOrDefault((TooltipState state) => { return state.currentText == text; }) != null)
        {
            return -1;
        }
        
        int index = CreateTooltip(text, onTooltipHide);
        
        if (!isChild)
        {
            _lastRootTooltipIndexOpened = index;
            //hide existing non-child tooltips
            for (int i = 0; i < _tooltips.Count; i++)
            {
                if (i != index && _tooltips[i].inUse && (_tooltips.FirstOrDefault((TooltipState state) => { return state.child == i; }) == null))
                {
                    HideTooltip(i);
                }
            }
        }

        UITooltip newTooltip = _tooltips[index].instantiatedTooltip;
        
        newTooltip.Populate(tooltipParent.InverseTransformPoint(tooltipWorldPos), text);
        
        return index;
    }

    public int ShowTooltipChild(Vector2 tooltipWorldPos, string text, Action onTooltipHide = null)
    {
        //tooltip children are always of the last opened tooltip
        int parentIndex = _lastRootTooltipIndexOpened;
        
        if (parentIndex == -1)
        {
            return ShowTooltip(tooltipWorldPos, text, onTooltipHide);
        }
        
        if (_tooltips[parentIndex].child != -1)
        {
            Debug.LogError("tried to create a child tooltip in a parent that already has a child!");
            return -1;
        }

        if (spawnChildrenOutsideOfParent)
        {
            int defaultTooltipPadding = _tooltips[parentIndex].instantiatedTooltip.GetDefaultVerticalPadding();
            
            Vector2 tooltipDirection = new Vector2(Screen.width / 2f, Screen.height / 2f) - tooltipWorldPos;
            if (Mathf.Abs(tooltipDirection.x) > Mathf.Abs(tooltipDirection.y))
            {
                tooltipDirection = new Vector2(tooltipDirection.x, 0);
            }
            else
            {
                tooltipDirection = new Vector2(0, tooltipDirection.y);
            }
            
            tooltipWorldPos = RectTransformUtils.GetPositionOutsideRectTransform(_tooltips[parentIndex].instantiatedTooltip.transform.GetChild(0).transform.GetChild(0).transform as RectTransform, 
                tooltipDirection,
                new Vector2(defaultTooltipPadding * Mathf.Sign(tooltipDirection.x) , defaultTooltipPadding * Mathf.Sign(tooltipDirection.y))
                );
        }
        
        int childIndex = ShowTooltip(tooltipWorldPos, text, onTooltipHide, isChild: true);
        
        _tooltips[parentIndex].child = childIndex;
        
        return childIndex;
    }

    public void HideTooltipIfMouseOff(int tooltipIndex)
    {
        if (tooltipIndex >= 0 && tooltipIndex < _tooltips.Count)
        {
            if (_tooltips[tooltipIndex].inUse && 
                !_tooltips[tooltipIndex].isHiding && 
                !_tooltips[tooltipIndex].tooltipAttemptHideCoroutine.IsRunning && 
                !_tooltips[tooltipIndex].instantiatedTooltip.IsMouseOnTooltip())
            {
                if (_tooltips[tooltipIndex].child == -1 || !_tooltips[_tooltips[tooltipIndex].child].instantiatedTooltip.IsMouseOnTooltip())
                {
                    Timing.RunCoroutineSingleton(TryHideTooltipIfMouseOff(tooltipIndex), _tooltips[tooltipIndex].tooltipAttemptHideCoroutine, SingletonBehavior.Overwrite);
                }
            }
        }
    }

    private IEnumerator<float> TryHideTooltipIfMouseOff(int tooltipIndex)
    {
        for (float t = 0; t < tooltipMouseOffHideTime; t += Time.deltaTime)
        {
            //if player re-entered tooltip area or that of a child with pointer, stop the hiding.
            if (!_tooltips[tooltipIndex].inUse ||
                _tooltips[tooltipIndex].isHiding || 
                _tooltips[tooltipIndex].instantiatedTooltip.IsMouseOnTooltip() ||
                (_tooltips[tooltipIndex].child != -1 &&
                 _tooltips[_tooltips[tooltipIndex].child].instantiatedTooltip.IsMouseOnTooltip()))
            {
                yield break;
            }
            
            yield return Timing.WaitForOneFrame;
        }
        
        HideTooltip(tooltipIndex);
    }

    public void HideTooltip(int tooltipIndex)
    {
        if (_tooltips[tooltipIndex].isHiding)
        {
            return;
        }
        
        if (_tooltips[tooltipIndex].child != -1)
        {
            //the child will set it's parent's reference to -1
            HideTooltip(_tooltips[tooltipIndex].child);
        }
        
        int indexOfMe = _tooltips.FindIndex((TooltipState tooltipState) => tooltipState.child == tooltipIndex);
        if (indexOfMe != -1)
        {
            _tooltips[indexOfMe].child = -1;
        }
        
        _tooltips[tooltipIndex].instantiatedTooltip.TearDown();
        
        _tooltips[tooltipIndex].isHiding = true;
        
        AsyncUtils.InvokeCallbackAfterSeconds(tooltipTearDownTime, () =>
        {
            _tooltips[tooltipIndex].inUse = false;
            _tooltips[tooltipIndex].currentText = string.Empty;
            _tooltips[tooltipIndex].onHideCallback?.Invoke();
            _tooltips[tooltipIndex].onHideCallback = null;
            _tooltips[tooltipIndex].isHiding = false;
            _tooltips[tooltipIndex].child = -1;
            _tooltips[tooltipIndex].instantiatedTooltip.gameObject.SetActive(false);
            Timing.KillCoroutines(_tooltips[tooltipIndex].tooltipAttemptHideCoroutine);
            _tooltips[tooltipIndex].tooltipAttemptHideCoroutine = new();
        });
    }

    private int CreateTooltip(string text, Action onTooltipHide)
    {
        int indexOfUnused = _tooltips.FindIndex((TooltipState state) =>
        {
            return !state.inUse;
        });
        
        if (indexOfUnused != -1)
        {
            _tooltips[indexOfUnused].inUse = true;
            _tooltips[indexOfUnused].currentText = text;
            _tooltips[indexOfUnused].instantiatedTooltip.gameObject.SetActive(true);
            _tooltips[indexOfUnused].instantiatedTooltip.transform.SetAsLastSibling();
            _tooltips[indexOfUnused].child = -1;
            _tooltips[indexOfUnused].onHideCallback = onTooltipHide;
            _tooltips[indexOfUnused].isHiding = false;
            Timing.KillCoroutines(_tooltips[indexOfUnused].tooltipAttemptHideCoroutine);
            _tooltips[indexOfUnused].tooltipAttemptHideCoroutine = new();
            
            return indexOfUnused;
        }
        else
        {
            _tooltips.Add(new TooltipState());
            _tooltips[^1].instantiatedTooltip = Instantiate(tooltipPrefab, tooltipParent);
            _tooltips[^1].instantiatedTooltip.transform.SetAsLastSibling();
            _tooltips[^1].inUse = true;
            _tooltips[^1].currentText = text;
            _tooltips[^1].child = -1;
            _tooltips[^1].onHideCallback = onTooltipHide;
            _tooltips[^1].isHiding = false;
            Timing.KillCoroutines(_tooltips[^1].tooltipAttemptHideCoroutine);
            _tooltips[^1].tooltipAttemptHideCoroutine = new();
            
            return _tooltips.Count - 1;
        }
    }
}
