using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SRF;
using UnityEngine;

public class ResourceIconObjectPool : MonoBehaviour
{
    [SerializeField] private RectTransform singleIconsRoot; 
    [SerializeField] private RectTransform doubleIconsRoot; 
    [SerializeField] private RectTransform tripleIconsRoot; 
    
    private List<RectTransform> _singleIcons = new List<RectTransform>();
    private List<RectTransform> _doubleIcons = new List<RectTransform>();
    private List<RectTransform> _tripleIcons = new List<RectTransform>();

    private List<RectTransform> _usedSingleIcons = new List<RectTransform>();
    private List<RectTransform> _usedDoubleIcons = new List<RectTransform>();
    private List<RectTransform> _usedTripleIcons = new List<RectTransform>();


    private void Awake()
    {
        _singleIcons.AddRange(singleIconsRoot.GetChildren().Cast<RectTransform>());
        _doubleIcons.AddRange(doubleIconsRoot.GetChildren().Cast<RectTransform>());
        _tripleIcons.AddRange(tripleIconsRoot.GetChildren().Cast<RectTransform>());
    }

    public RectTransform GetIcon(int numIcons)
    {
        RectTransform newIcon = null;
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

    public void ReturnIcon(RectTransform icon, int numIcons)
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

    private void ReturnIconToList(RectTransform icon, List<RectTransform> list, RectTransform parent)
    {
        icon.SetParent(parent);
        list.Add(icon);
    }

    private RectTransform GetIconFromList(List<RectTransform> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("Error when trying to get icon from list. no icons are left! check the object pool quantities for resource icons.");
            return null;
        }
        else
        {
            RectTransform newIcon = list[^1];
            
            newIcon.SetParent(null);
            
            list.RemoveAt(list.Count - 1);
            return newIcon;
        }
    }
}
