using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomChanceSystem : Singleton<RandomChanceSystem>
{

    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    private int _currentSeed;

    private void Awake()
    {
        //TODO: Temp, make an actual game start thing.
        OnGameStart();
    }

    private void OnGameStart()
    {
        _currentSeed = seed;
        if (useRandomSeed)
        {
            _currentSeed = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue);
        }
    }

    public int GetCurrentSeed()
    {
        return _currentSeed;
    }

    public List<RelicTypes> GenerateRelicTypesInShop(int numRelics, int numRefreshes)
    {
        //create a random seed based on the current global seed, the current turn number, and number of refreshes

        int seed = _currentSeed + 3605 * RoundState.Instance.RoundNumber + 2821 *  numRefreshes;
        
        Random.InitState(seed);
        
        List<RelicTypes> unownedRelicTypes = new List<RelicTypes>();

        List<RelicTypes> ownedRelicTypes = RelicSystem.Instance.GetOwnedRelics();
        foreach (RelicTypes relicType in Enum.GetValues(typeof(RelicTypes)))
        {
            if (!ownedRelicTypes.Contains(relicType))
            {
                unownedRelicTypes.Add(relicType);
            }
        }

        List<RelicTypes> selectedRelics = new();

        int maxRelicValue = Enum.GetValues(typeof(RelicTypes)).Length;

        for (int i = 0; i < numRelics; i++)
        {
            int newRelic = Random.Range(1, maxRelicValue);

            if (selectedRelics.Contains((RelicTypes)newRelic)) 
            {
                for (int j = 0; j < maxRelicValue - 1; j++)
                {
                    int tryRelic = (newRelic + j) % (maxRelicValue - 1) + 1;
                    if (!selectedRelics.Contains((RelicTypes) tryRelic))
                    {
                        newRelic = tryRelic;
                        break;
                    }
                }
            }
            
            selectedRelics.Add((RelicTypes)newRelic);
        }
        
        //reset the seed
        Random.InitState((int)DateTime.Now.Ticks);

        return selectedRelics;
    }
}
