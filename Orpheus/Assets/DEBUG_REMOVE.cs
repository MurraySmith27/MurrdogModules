using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEBUG_REMOVE : MonoBehaviour
{
    private TMP_Text text;
    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    void Update()
    {
        text.SetText($"Pay <color=#FFD739><sprite=0>7</color> in <color=#EF2847>{3 - PersistentState.Instance.HarvestNumber}</color> turns");
    }
}
