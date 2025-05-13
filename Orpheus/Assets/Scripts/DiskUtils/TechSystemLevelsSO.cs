using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum TechSystemRewardType
{
    CITIZENS,
    HANDS,
    DISCARDS,
    UPGRADE_SLOTS,
    UNLOCK_BUILDINGS,
}

[CreateAssetMenu(fileName = "TechSystemLevels.asset", menuName = "Orpheus/Tech System Levels")]
public class TechSystemLevelsSO : ScriptableObject
{
    [Serializable]
    public class TechSystemLevel
    {
        [Serializable]
        public class TechSystemLevelReward
        {
            public TechSystemRewardType Type;
            public int Quantity; 
            public List<BuildingType> BuildingTypes = new List<BuildingType>();
        }
        
        public int XpForLevel;
        public List<TechSystemLevelReward> Rewards;
    }

    [SerializeField] private List<TechSystemLevel> Levels;

    public int GetCurrentLevel(int xpTotal)
    {
        int currentXp = xpTotal;

        int i;
        for (i = 0; i < Levels.Count; i++)
        {
            if (currentXp >= Levels[i].XpForLevel)
            {
                currentXp -= Levels[i].XpForLevel;
            }
            else break;
        }

        return i + 1;
    }
    
    public int GetExpOfCurrentLevel(int xpTotal)
    {
        int currentXp = xpTotal;

        int total = 0;

        int i = 0;
        for (i = 0; i < Levels.Count; i++)
        {
            if (currentXp >= Levels[i].XpForLevel)
            {
                currentXp -= Levels[i].XpForLevel;
            }
            else break;
        }

        return currentXp;
    }

    public int GetExpUntilNextLevel(int xpTotal)
    {
        int currentXp = xpTotal;

        int total = 0;

        int i = 0;
        for (i = 0; i < Levels.Count; i++)
        {
            if (currentXp >= Levels[i].XpForLevel)
            {
                currentXp -= Levels[i].XpForLevel;
            }
            else return Levels[i].XpForLevel - currentXp;
        }

        return -1;
    }
    

    public int GetTotalTechSystemCitizens(int xpTotal)
    {
        return GetAllIntRewardsOfType(xpTotal, TechSystemRewardType.CITIZENS);
    }
    
    public int GetTotalTechSystemHands(int xpTotal)
    {
        return GetAllIntRewardsOfType(xpTotal, TechSystemRewardType.HANDS);
    }
    
    public int GetTotalTechSystemDiscards(int xpTotal)
    {
        return GetAllIntRewardsOfType(xpTotal, TechSystemRewardType.DISCARDS);
    }

    public int GetTotalTechSystemUpgradeSlots(int xpTotal)
    {
        return GetAllIntRewardsOfType(xpTotal, TechSystemRewardType.UPGRADE_SLOTS);
    }

    public int GetRewardOfLevel(int level, TechSystemRewardType rewardType)
    {
        if (level == 0 || level > Levels.Count + 1)
        {
            return 0;
        }

        int indexOfRewardType = Levels[level - 1].Rewards.FindIndex((reward) =>
        {
            return reward.Type == rewardType;
        });

        if (indexOfRewardType != -1)
        {
            return Levels[level - 1].Rewards[indexOfRewardType].Quantity;
        }
        else return 0;
    }

    public List<BuildingType> GetUnlockedBuildingsOfLevel(int level)
    {
        if (level == 0 || level > Levels.Count + 1)
        {
            return new();
        }

        int indexOfRewardType = Levels[level - 1].Rewards.FindIndex((reward) =>
        {
            return reward.Type == TechSystemRewardType.UNLOCK_BUILDINGS;
        });

        if (indexOfRewardType != -1)
        {
            return Levels[level - 1].Rewards[indexOfRewardType].BuildingTypes;
        }
        else return new();
    }
    
    private int GetAllIntRewardsOfType(int xpTotal, TechSystemRewardType type)
    {
        int currentXp = xpTotal;

        int total = 0;

        int i = 0;
        for (i = 0; i < Levels.Count; i++)
        {
            if (currentXp >= Levels[i].XpForLevel)
            {
                foreach (TechSystemLevel.TechSystemLevelReward reward in Levels[i].Rewards)
                {
                    if (reward.Type == type)
                    {
                        total += reward.Quantity;   
                    }
                }
                
                currentXp -= Levels[i].XpForLevel;
            }
            else break;
        }

        return total;
    }
    
    public List<BuildingType> GetAllUnlockedBuildingTypes(int xpTotal)
    {
        int currentXp = xpTotal;
        
        List<BuildingType> unlockedBuildings = new List<BuildingType>();

        int i = 0;
        for (i = 0; i < Levels.Count; i++)
        {
            if (currentXp >= Levels[i].XpForLevel)
            {
                foreach (TechSystemLevel.TechSystemLevelReward reward in Levels[i].Rewards)
                {
                    if (reward.Type == TechSystemRewardType.UNLOCK_BUILDINGS)
                    {
                        unlockedBuildings.AddRange(reward.BuildingTypes);   
                    }
                }
                
                currentXp -= Levels[i].XpForLevel;
            }
            else break;
        }

        return unlockedBuildings;
    }
}
