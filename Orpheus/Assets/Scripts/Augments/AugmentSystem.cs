using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AugmentTypes
{
    NONE,
    FOIL,
    HOLO,
    GOLDEN
}

public class AugmentSystem : Singleton<AugmentSystem> 
{
    private Dictionary<AugmentTypes, Augment> _augmentInstances = new Dictionary<AugmentTypes, Augment>();
    
    public event Action<AugmentTypes> OnAugmentAdded;
    public event Action<AugmentTypes> OnAugmentRemoved;
    public event Action<AugmentTypes, AdditionalTriggeredArgs> OnAugmentTriggered;
    
    
    private void Awake()
    {
        AugmentFactory augmentFactory = new AugmentFactory();
        
        for (int i = 0; i < Enum.GetValues(typeof(AugmentTypes)).Length; i++)
        {
            _augmentInstances.Add((AugmentTypes)i, augmentFactory.CreateAugment((AugmentTypes)i));
        }
    }
    
    public virtual List<Augment.AugmentTriggeredData> OnTileTriggered(Vector2Int tilePosition)
    {
        List<Augment.AugmentTriggeredData> triggeredAugmentData = new();
        
        List<AugmentTypes> augmentTypesOnTile = MapSystem.Instance.GetAugmentsOnTile(tilePosition);

        foreach (AugmentTypes augmentType in augmentTypesOnTile)
        {
            if (_augmentInstances[augmentType].OnTileTriggered(tilePosition, out Augment.AugmentTriggeredData triggeredData))
            {
                triggeredAugmentData.Add(triggeredData);
            }   
        }

        return triggeredAugmentData;
    }

    public bool OnTileMaintenanceCostComputed(Vector2Int tilePosition, int maintenanceCostOfThisTile, out int newMaintenanceCostOfThisTile)
    {
        newMaintenanceCostOfThisTile = maintenanceCostOfThisTile;

        List<AugmentTypes> augmentTypesOnTile = MapSystem.Instance.GetAugmentsOnTile(tilePosition);

        bool triggered = false;
        
        foreach (AugmentTypes augmentType in augmentTypesOnTile)
        {
            int newMaintenanceCost;
            if (_augmentInstances[augmentType]
                .OnTileMaintenanceCostComputed(tilePosition, newMaintenanceCostOfThisTile, out newMaintenanceCost))
            {
                triggered = true;
                newMaintenanceCostOfThisTile = newMaintenanceCost;
            }
        }

        return triggered;
    }
}
