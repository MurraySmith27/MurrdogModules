using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldText : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void Start()
    {
        PersistentState.Instance.OnGoldValueChanged -= SetGoldText;
        PersistentState.Instance.OnGoldValueChanged += SetGoldText;
        
        SetGoldText(PersistentState.Instance.CurrentGold);
    }

    private void OnEnable()
    {
        SetGoldText(PersistentState.Instance.CurrentGold);
    }

    private void OnDestroy()
    {
        if (PersistentState.IsAvailable)
        {
            PersistentState.Instance.OnGoldValueChanged -= SetGoldText;
        }
    }

    private void SetGoldText(long gold)
    {
        goldText.SetText($"<sprite index=0><wiggle a=0.1 f=0.5>{PersistentState.Instance.CurrentGold}</wiggle>");
    }
}
