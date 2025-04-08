using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class SubmenuManager : Singleton<MonoBehaviour>
{
    private class SubmenuState
    {
        public UISubmenu instantiatedSubmenu;
        public int child;
        public bool inUse;
        public string currentText;
        public Action onHideCallback;
        public bool isHiding;
        public CoroutineHandle submenuAttemptHideCoroutine;
    }
    
    [SerializeField] private UISubmenu submenuPrefab;

    [SerializeField] private Transform submenuParent;

    [SerializeField] private float submenuTearDownTime = 0.2f;

    [SerializeField] private float submenuMouseOffHideTime = 0.2f;
    
    private List<SubmenuState> _submenus = new List<SubmenuState>();

    private int _lastRootSubmenuIndexOpened = -1;

    public int ShowSubmenuById(Vector2 submenuWorldPosition, string submenuId, Action onSubmenuHide = null, bool isChild = false)
    {
        string submenuText;
        if (submenuDataSO.GetSubmenuTextFromId(submenuId, out submenuText))
        {
            return ShowSubmenu(submenuWorldPosition, submenuText, onSubmenuHide, isChild);
        }
        else
        {
            Debug.LogError("Can't find submenu with id: " + submenuId);
            return -1;
        }
    }

    public int ShowSubmenuChildBySubmenuId(Vector2 submenuWorldPosition, string submenuId, Action onSubmenuHide = null,
        bool isChild = false)
    {
        string submenuText;
        if (submenuDataSO.GetSubmenuTextFromId(submenuId, out submenuText))
        {
            return ShowSubmenuChild(submenuWorldPosition, submenuText, onSubmenuHide);
        }
        else
        {
            Debug.LogError("Can't find submenu with id: " + submenuId);
            return -1;
        }
    }
    
    public int ShowSubmenu(Vector2 submenuWorldPos, string text, Action onSubmenuHide = null, bool isChild = false)
    {
        if (_submenus.FirstOrDefault((SubmenuState state) => { return state.currentText == text; }) != null)
        {
            return -1;
        }
        
        int index = CreateSubmenu(text, onSubmenuHide);
        
        if (!isChild)
        {
            _lastRootSubmenuIndexOpened = index;
            //hide existing non-child submenus
            for (int i = 0; i < _submenus.Count; i++)
            {
                if (i != index && _submenus[i].inUse && (_submenus.FirstOrDefault((SubmenuState state) => { return state.child == i; }) == null))
                {
                    HideSubmenu(i);
                }
            }
        }

        UISubmenu newSubmenu = _submenus[index].instantiatedSubmenu;
        
        newSubmenu.Populate(submenuParent.InverseTransformPoint(submenuWorldPos), text);
        
        return index;
    }

    public int ShowSubmenuChild(Vector2 submenuWorldPos, string text, Action onSubmenuHide = null)
    {
        //submenu children are always of the last opened submenu
        int parentIndex = _lastRootSubmenuIndexOpened;
        
        if (parentIndex == -1)
        {
            return ShowSubmenu(submenuWorldPos, text, onSubmenuHide);
        }
        
        if (_submenus[parentIndex].child != -1)
        {
            Debug.LogError("tried to create a child submenu in a parent that already has a child!");
            return -1;
        }
        
        int childIndex = ShowSubmenu(submenuWorldPos, text, onSubmenuHide, isChild: true);
        
        _submenus[parentIndex].child = childIndex;
        
        return childIndex;
    }

    public void HideSubmenuIfMouseOff(int submenuIndex)
    {
        if (submenuIndex >= 0 && submenuIndex < _submenus.Count)
        {
            if (_submenus[submenuIndex].inUse && 
                !_submenus[submenuIndex].isHiding && 
                !_submenus[submenuIndex].submenuAttemptHideCoroutine.IsRunning && 
                !_submenus[submenuIndex].instantiatedSubmenu.IsMouseOnSubmenu())
            {
                if (_submenus[submenuIndex].child == -1 || !_submenus[_submenus[submenuIndex].child].instantiatedSubmenu.IsMouseOnSubmenu())
                {
                    Timing.RunCoroutineSingleton(TryHideSubmenuIfMouseOff(submenuIndex), _submenus[submenuIndex].submenuAttemptHideCoroutine, SingletonBehavior.Overwrite);
                }
            }
        }
    }

    private IEnumerator<float> TryHideSubmenuIfMouseOff(int submenuIndex)
    {
        for (float t = 0; t < submenuMouseOffHideTime; t += Time.deltaTime)
        {
            //if player re-entered submenu area or that of a child with pointer, stop the hiding.
            if (!_submenus[submenuIndex].inUse ||
                _submenus[submenuIndex].isHiding || 
                _submenus[submenuIndex].instantiatedSubmenu.IsMouseOnSubmenu() ||
                (_submenus[submenuIndex].child != -1 &&
                 _submenus[_submenus[submenuIndex].child].instantiatedSubmenu.IsMouseOnSubmenu()))
            {
                yield break;
            }
            
            yield return Timing.WaitForOneFrame;
        }
        
        HideSubmenu(submenuIndex);
    }

    public void HideSubmenu(int submenuIndex)
    {
        if (_submenus[submenuIndex].isHiding)
        {
            return;
        }
        
        if (_submenus[submenuIndex].child != -1)
        {
            //the child will set it's parent's reference to -1
            HideSubmenu(_submenus[submenuIndex].child);
        }
        
        int indexOfMe = _submenus.FindIndex((SubmenuState submenuState) => submenuState.child == submenuIndex);
        if (indexOfMe != -1)
        {
            _submenus[indexOfMe].child = -1;
        }
        
        _submenus[submenuIndex].instantiatedSubmenu.TearDown();
        
        _submenus[submenuIndex].isHiding = true;
        
        AsyncUtils.InvokeCallbackAfterSeconds(submenuTearDownTime, () =>
        {
            _submenus[submenuIndex].inUse = false;
            _submenus[submenuIndex].currentText = string.Empty;
            _submenus[submenuIndex].onHideCallback?.Invoke();
            _submenus[submenuIndex].onHideCallback = null;
            _submenus[submenuIndex].isHiding = false;
            _submenus[submenuIndex].child = -1;
            _submenus[submenuIndex].instantiatedSubmenu.gameObject.SetActive(false);
            Timing.KillCoroutines(_submenus[submenuIndex].submenuAttemptHideCoroutine);
            _submenus[submenuIndex].submenuAttemptHideCoroutine = new();
        });
    }

    private int CreateSubmenu(string text, Action onSubmenuHide)
    {
        int indexOfUnused = _submenus.FindIndex((SubmenuState state) =>
        {
            return !state.inUse;
        });
        
        if (indexOfUnused != -1)
        {
            _submenus[indexOfUnused].inUse = true;
            _submenus[indexOfUnused].currentText = text;
            _submenus[indexOfUnused].instantiatedSubmenu.gameObject.SetActive(true);
            _submenus[indexOfUnused].child = -1;
            _submenus[indexOfUnused].onHideCallback = onSubmenuHide;
            _submenus[indexOfUnused].isHiding = false;
            Timing.KillCoroutines(_submenus[indexOfUnused].submenuAttemptHideCoroutine);
            _submenus[indexOfUnused].submenuAttemptHideCoroutine = new();
            
            return indexOfUnused;
        }
        else
        {
            _submenus.Add(new SubmenuState());
            _submenus[^1].instantiatedSubmenu = Instantiate(submenuPrefab, submenuParent); 
            _submenus[^1].inUse = true;
            _submenus[^1].currentText = text;
            _submenus[^1].child = -1;
            _submenus[^1].onHideCallback = onSubmenuHide;
            _submenus[^1].isHiding = false;
            Timing.KillCoroutines(_submenus[^1].submenuAttemptHideCoroutine);
            _submenus[^1].submenuAttemptHideCoroutine = new();
            
            return _submenus.Count - 1;
        }
    }
}
