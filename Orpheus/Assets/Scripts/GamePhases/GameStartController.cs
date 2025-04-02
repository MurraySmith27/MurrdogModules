using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStartController : Singleton<GameStartController>
{
    public event Action OnGameStart;
    
    public void StartGame()
    {
        OnGameStart?.Invoke();
    }
}
