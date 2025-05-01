using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersistentResourceText : MonoBehaviour
{
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private PersistentResourceType resourceType;
    void Start()
    {
        switch (resourceType)
        {
            case PersistentResourceType.Stone:
                PersistentState.Instance.OnStoneValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnStoneValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentStone);
                break;
            case PersistentResourceType.Copper:
                PersistentState.Instance.OnCopperValueChanged += OnResourceValueChanged;
                PersistentState.Instance.OnCopperValueChanged -= OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentCopper);
                break;
            case PersistentResourceType.Steel:
                PersistentState.Instance.OnSteelValueChanged += OnResourceValueChanged;
                PersistentState.Instance.OnSteelValueChanged -= OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentSteel);
                break;
            case PersistentResourceType.Wood:
                PersistentState.Instance.OnWoodValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnWoodValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentWood);
                break;
            case PersistentResourceType.Lumber:
                PersistentState.Instance.OnLumberValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnLumberValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentLumber);
                break;
            case PersistentResourceType.Gold:
                PersistentState.Instance.OnGoldValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnGoldValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentGold);
                break;
        }
    }

    private void OnDestroy()
    {
        if (PersistentState.IsAvailable)
        {
            switch (resourceType)
            {
                case PersistentResourceType.Stone:
                    PersistentState.Instance.OnStoneValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Copper:
                    PersistentState.Instance.OnCopperValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Steel:
                    PersistentState.Instance.OnSteelValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Wood:
                    PersistentState.Instance.OnWoodValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Lumber:
                    PersistentState.Instance.OnLumberValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Gold:
                    PersistentState.Instance.OnGoldValueChanged -= OnResourceValueChanged;
                    break;
            }
        }
    }

    private void OnResourceValueChanged(long newResourceValue)
    {
        resourceText.SetText($"{newResourceValue}");
    }
}
