using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedChanged : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    public void OnGameSpeedInputChanged()
    {
        float gameSpeed = float.Parse(inputField.text);
        
        GlobalSettings.GameSpeed = gameSpeed;
    }

    public void Start()
    {
        GlobalSettings.GameSpeed = 1f;
    }
}
