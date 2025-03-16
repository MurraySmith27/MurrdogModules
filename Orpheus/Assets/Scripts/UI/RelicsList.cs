using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicsList : MonoBehaviour
{

    [SerializeField] private RelicIcon relicIconPrefab;

    [SerializeField] private Transform relicListParent;

    private Dictionary<RelicTypes, RelicIcon> _instantiatedRelics = new Dictionary<RelicTypes, RelicIcon>();
    
    private void Start()
    {
        RelicSystem.Instance.OnRelicAdded -= OnRelicAdded;
        RelicSystem.Instance.OnRelicAdded += OnRelicAdded;
        
        RelicSystem.Instance.OnRelicRemoved -= OnRelicRemoved;
        RelicSystem.Instance.OnRelicRemoved += OnRelicRemoved;
    }

    private void OnDestroy()
    {
        if (RelicSystem.IsAvailable)
        {
            RelicSystem.Instance.OnRelicAdded -= OnRelicAdded;
            RelicSystem.Instance.OnRelicRemoved -= OnRelicRemoved;
        }
    }

    private void OnRelicAdded(RelicTypes relicType)
    {
        if (_instantiatedRelics.ContainsKey(relicType))
        {
            if (_instantiatedRelics[relicType] != null)
            {
                Destroy(_instantiatedRelics[relicType].gameObject);
            }
        }
        
        _instantiatedRelics[relicType] = Instantiate(relicIconPrefab, relicListParent);
        
        _instantiatedRelics[relicType].SetIcon(relicType);
    }
    
    private void OnRelicRemoved(RelicTypes relicType)
    {
        if (_instantiatedRelics.ContainsKey(relicType))
        {
            if (_instantiatedRelics[relicType] != null)
            {
                Destroy(_instantiatedRelics[relicType].gameObject);
            }
            
            _instantiatedRelics.Remove(relicType);
        }
    }
}
