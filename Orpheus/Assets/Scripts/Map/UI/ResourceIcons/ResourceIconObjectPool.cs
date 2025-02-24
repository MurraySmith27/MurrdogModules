using System.Collections;
using System.Collections.Generic;
using SRF;
using UnityEngine;

public class ResourceIconObjectPool : MonoBehaviour
{
    [SerializeField] private Transform singleIconsRoot; 
    [SerializeField] private Transform doubleIconsRoot; 
    [SerializeField] private Transform tripleIconsRoot; 
    
    private List<Transform> _singleIcons = new List<Transform>();
    private List<Transform> _doubleIcons = new List<Transform>();
    private List<Transform> _tripleIcons = new List<Transform>();

    private List<Transform> _usedSingleIcons = new List<Transform>();
    private List<Transform> _usedDoubleIcons = new List<Transform>();
    private List<Transform> _usedTripleIcons = new List<Transform>();


    private void Awake()
    {
        _singleIcons.AddRange(singleIconsRoot.GetChildren());
        _doubleIcons.AddRange(doubleIconsRoot.GetChildren());
        _tripleIcons.AddRange(tripleIconsRoot.GetChildren());
    }

    public Transform GetIcon(int numIcons)
    {
        Transform newIcon = null;
        switch (numIcons)
        {
            case 1:
                newIcon = GetIconFromList(_singleIcons);
                _usedSingleIcons.Add(newIcon);
                return newIcon;
            case 2:
                newIcon = GetIconFromList(_doubleIcons);
                _usedDoubleIcons.Add(newIcon);
                return newIcon;
            case 3:
                newIcon = GetIconFromList(_tripleIcons);
                _usedTripleIcons.Add(newIcon);
                return newIcon;
            default:
                Debug.LogError($"Cannot fetch a resource icon from object pool of quantity: {numIcons}");
                return null;
        }
    }

    public void ReturnIcon(Transform icon, int numIcons)
    {
        switch (numIcons)
        {
            case 1:
                int singleIndex = _usedSingleIcons.IndexOf(icon);
                if (singleIndex != -1)
                {
                    _usedSingleIcons.RemoveAt(singleIndex);
                    ReturnIconToList(icon, _singleIcons, singleIconsRoot);
                }
                break;
            case 2:
                int doubleIndex = _usedDoubleIcons.IndexOf(icon);
                if (doubleIndex != -1)
                {
                    _usedDoubleIcons.RemoveAt(doubleIndex);
                    ReturnIconToList(icon, _doubleIcons, doubleIconsRoot);
                }
                break;
            case 3:
                int tripleIndex = _usedTripleIcons.IndexOf(icon);
                if (tripleIndex != -1)
                {
                    _usedTripleIcons.RemoveAt(tripleIndex);
                    ReturnIconToList(icon, _tripleIcons, tripleIconsRoot);
                }
                break;
        }
    }

    private void ReturnIconToList(Transform icon, List<Transform> list, Transform parent)
    {
        icon.SetParent(parent);
        list.Add(icon);
    }

    private Transform GetIconFromList(List<Transform> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("Error when trying to get icon from list. no icons are left! check the object pool quantities for resource icons.");
            return null;
        }
        else
        {
            Transform newIcon = list[^1];
            
            newIcon.SetParent(null);
            
            list.RemoveAt(list.Count - 1);
            return newIcon;
        }
    }
}
