using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechSystem : Singleton<TechSystem>
{
    [SerializeField] private TechSystemLevelsSO techSystemLevelsSO;
    
    private int _currentExp = 0;

    public event Action OnExpChanged;

    public event Action<int> OnLevelUp;

    private void Start()
    {
        GameStartController.Instance.OnGameStart -= OnGameStart;
        GameStartController.Instance.OnGameStart += OnGameStart;
    }
    
    private void OnDestroy()
    {
        if (GameStartController.IsAvailable)
        {
            GameStartController.Instance.OnGameStart -= OnGameStart;
        }
    }

    private void OnGameStart()
    {
        _currentExp = 0;
    }
    
    public void AddExp(int quantity)
    {
        int currentLevel = techSystemLevelsSO.GetCurrentLevel(_currentExp);
        _currentExp += quantity;
        OnExpChanged?.Invoke();

        int newLevel = techSystemLevelsSO.GetCurrentLevel(_currentExp);


        for (int i = 0; i < newLevel - currentLevel; i++)
        {
            OnLevelUp?.Invoke(currentLevel + i);
        }
    }

    public int GetRewardOfLevel(int level, TechSystemRewardType rewardType)
    {
        return techSystemLevelsSO.GetRewardOfLevel(level, rewardType);
    }
    
    public List<BuildingType> GetUnlockedBuildingsOfLevel(int level)
    {
        return techSystemLevelsSO.GetUnlockedBuildingsOfLevel(level);
    }

    public int GetExpOfCurrentLevel()
    {
        return techSystemLevelsSO.GetExpOfCurrentLevel(_currentExp);
    }

    public int GetExpUntilNextLevel()
    {
        return techSystemLevelsSO.GetExpUntilNextLevel(_currentExp);
    }

    public int GetCurrentLevel()
    {
        return techSystemLevelsSO.GetCurrentLevel(_currentExp);
    }

    public List<BuildingType> GetUnlockedBuildings()
    {
        return techSystemLevelsSO.GetAllUnlockedBuildingTypes(_currentExp);
    }
}
