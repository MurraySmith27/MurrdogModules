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
            case PersistentResourceType.Wood:
                PersistentState.Instance.OnWoodValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnWoodValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentWood);
                break;
            case PersistentResourceType.Gold:
                PersistentState.Instance.OnGoldValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnGoldValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentGold);
                break;
            case PersistentResourceType.BuildToken:
                PersistentState.Instance.OnBuildTokensValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnBuildTokensValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentBuildTokens);
                break;
            case PersistentResourceType.Water:
                PersistentState.Instance.OnWaterValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnWaterValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentWater);
                break;
            case PersistentResourceType.Dirt:
                PersistentState.Instance.OnDirtValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnDirtValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentDirt);
                break;
            case PersistentResourceType.Oil:
                PersistentState.Instance.OnOilValueChanged -= OnResourceValueChanged;
                PersistentState.Instance.OnOilValueChanged += OnResourceValueChanged;
                OnResourceValueChanged(PersistentState.Instance.CurrentOil);
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
                case PersistentResourceType.Wood:
                    PersistentState.Instance.OnWoodValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Gold:
                    PersistentState.Instance.OnGoldValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.BuildToken:
                    PersistentState.Instance.OnBuildTokensValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Water:
                    PersistentState.Instance.OnWaterValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Dirt:
                    PersistentState.Instance.OnDirtValueChanged -= OnResourceValueChanged;
                    break;
                case PersistentResourceType.Oil:
                    PersistentState.Instance.OnOilValueChanged -= OnResourceValueChanged;
                    break;
            }
        }
    }

    private void OnResourceValueChanged(long newResourceValue)
    {
        resourceText.SetText($"{newResourceValue}");
    }
}
