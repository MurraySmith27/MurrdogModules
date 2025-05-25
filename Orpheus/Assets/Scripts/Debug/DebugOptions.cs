#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class DebugOptions
{
    public static readonly DebugOptions Instance = new DebugOptions();

    [Category("Camera Testing")]
    public void FocusCameraOnZeroZeroTile()
    {
        CameraController.Instance.FocusPosition(new Vector3(0, 0, 0));
    }

    public void IncrementRoundNumber()
    {
        HarvestState.Instance.SetFoodGoalForHarvest(PersistentState.Instance.HarvestNumber);
        
        PersistentState.Instance.IncrementHarvestNumber();
        
        HarvestState.Instance.ResetFoodScore();
    }

    public void AddLotsOfBuildingTokens()
    {
        PersistentState.Instance.ChangeCurrentBuildTokens(10);
    }

    public void AddSmallExp()
    {
        TechSystem.Instance.AddExp(3);
    }
    
    public void AddAllExp()
    {
        TechSystem.Instance.AddExp(500);
    }
    
    public void AddLotsOfGold()
    {
        PersistentState.Instance.ChangeCurrentGold(10000);    
    }

    public void AddLotsOfWaterDirtAndOil()
    {
        PersistentState.Instance.ChangeCurrentWater(300);
        PersistentState.Instance.ChangeCurrentDirt(300);
        PersistentState.Instance.ChangeCurrentOil(300);
        
    }
    
    private BuildingType _buildingType = BuildingType.Well;

    public BuildingType BuildingType
    {
        get { return _buildingType; }
        set
        {
            _buildingType = value;
        }
    }
    
    public void PlaceSelectedBuilding()
    {
        MapInteractionController.Instance.SwitchToPlaceBuildingMode(BuildingType);
    }
    
    private RelicTypes _relicType = RelicTypes.COW_PLUSHIE;
    
    public RelicTypes RelicType
    {
        get { return _relicType; }
        set
        {
            _relicType = value;
        }
    }
    
    public void AddSelectedRelic()
    {
        RelicSystem.Instance.AddRelic(RelicType);
    }
    
    [Category("Game Speed")]
    public void DoubleGameSpeed()
    {
        GlobalSettings.GameSpeed *= 2;
    }
    
    [Category("Game Speed")]
    public void ResetGameSpeed()
    {
        GlobalSettings.GameSpeed = 1;
    }

    
}
#endif
