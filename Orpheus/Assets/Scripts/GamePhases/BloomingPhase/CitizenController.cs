using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class CitizenController : Singleton<CitizenController>
{
    public delegate void OnHandUsedDelegate(Dictionary<Guid, List<CitizenPlacement>> citizenPlacementsPerCity);

    public event OnHandUsedDelegate OnHandUsed;
    
    public delegate void OnDiscardUsedDelegate(Guid cityGuid, List<CitizenPlacement> citizenPlacementsBefore, List<CitizenPlacement> citizenPlacementsAfter);

    public event OnDiscardUsedDelegate OnDiscardUsed;

    public delegate void CitizenOnTileDelegate(Guid cityGuid, Vector2Int placedTile);
    
    public event CitizenOnTileDelegate OnBonusCitizenUsed;

    public event CitizenOnTileDelegate OnCitizenAddedToTile;
    
    public event CitizenOnTileDelegate OnCitizenRemovedFromTile;

    public event Action<Vector2Int> OnCitizenLocked;
    
    public event Action<Vector2Int> OnCitizenUnlocked;
    
    public class CitizenPlacement
    {
        public CitizenPlacement(Vector2Int _position, bool _isLocked)
        {
            Position = _position;
            IsLocked = _isLocked;
        }
        
        public Vector2Int Position;
        public bool IsLocked;
    }
    
    private Dictionary<Guid, List<CitizenPlacement>> _citizenPlacements = new();

    private void Start()
    {
        HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
        HarvestState.Instance.OnHarvestStart += OnHarvestStart;

        HarvestState.Instance.OnFoodGoalReached -= OnHarvestEnd;
        HarvestState.Instance.OnFoodGoalReached += OnHarvestEnd;

        PhaseStateMachine.Instance.OnPhaseTransitionStarted -= OnPhaseTransitionStarted;
        PhaseStateMachine.Instance.OnPhaseTransitionStarted += OnPhaseTransitionStarted;

        BloomingHarvestController.Instance.OnTileBonusTickEnd -= OnTileBonusTickEnd;
        BloomingHarvestController.Instance.OnTileBonusTickEnd += OnTileBonusTickEnd;
    }

    private void OnDestroy()
    {
        if (HarvestState.IsAvailable)
        {
            HarvestState.Instance.OnHarvestStart -= OnHarvestStart;
            HarvestState.Instance.OnFoodGoalReached -= OnHarvestEnd;

        }

        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseTransitionStarted -= OnPhaseTransitionStarted;
        }

        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnTileBonusTickEnd -= OnTileBonusTickEnd;
        }
    }

    private void OnPhaseTransitionStarted(GamePhases gamePhases)
    {
        if (gamePhases == GamePhases.BloomingHarvestTurn)
        {
            Reset();
            
            List<Guid> allCityGuids = MapSystem.Instance.GetAllCityGuids();

            if (allCityGuids.Count > 0)
            {
                TryPlaceCitizensOnUnoccupiedTiles(allCityGuids[0], HarvestState.Instance.NumCitizensUsedThisHarvest,
                    out List<Vector2Int> chosenTiles);
            }            
        }
    }
    
    private void OnHarvestStart()
    {
    }

    private void OnTileBonusTickEnd(Vector2Int position)
    {
        CitizenPlacement placement;

        foreach (Guid cityGuid in _citizenPlacements.Keys)
        {
            placement = _citizenPlacements[cityGuid].FirstOrDefault((CitizenPlacement citizenPlacement) =>
            {
                return citizenPlacement.Position == position;
            });

            if (placement != null)
            {
                _citizenPlacements[cityGuid].Remove(placement);
                OnCitizenRemovedFromTile?.Invoke(cityGuid, placement.Position);
                break;
            }
        }
    }

    private void OnHarvestEnd()
    {
        Reset();
    }
    
    //this method assumes there is a bonus citizen item being used.
    public bool TryPlaceBonusCitizenOnRandomTile(Guid cityGuid)
    {
        if (TryPlaceCitizensOnUnoccupiedTiles(cityGuid, 1, out List<Vector2Int> chosenTiles))
        {
            HarvestState.Instance.UseCitizens(1);
            
            OnBonusCitizenUsed?.Invoke(cityGuid, chosenTiles[0]);
            return true;
        }
        else
        {
            Debug.LogError("Tried to place a bonus citizen and couldn't!");
            return false;
        }
    
    }

    private bool TryPlaceCitizensOnUnoccupiedTiles(Guid cityGuid, int numCitizensToPlace, out List<Vector2Int> chosenTiles)
    {
        chosenTiles = new();
        
        List<Vector2Int> allOwnedCityTiles = MapSystem.Instance.GetOwnedTilesOfCity(cityGuid);
        
        List<Vector2Int> unoccupiedCityTiles;
        if (_citizenPlacements.ContainsKey(cityGuid))
        {
            unoccupiedCityTiles = allOwnedCityTiles.Where((Vector2Int position) =>
            {
                return _citizenPlacements[cityGuid].FirstOrDefault((CitizenPlacement citizenPlacement) =>
                {
                    return citizenPlacement.Position == position;
                }) == null;
            }).ToList();
        }
        else
        {
            unoccupiedCityTiles = allOwnedCityTiles;
        }
        
        for (int i = 0; i < numCitizensToPlace; i++)
        {
            if (unoccupiedCityTiles.Count >= 0)
            {
                Vector2Int tile =
                    RandomChanceSystem.Instance.GetNextCitizenTile(unoccupiedCityTiles);
                
                unoccupiedCityTiles.Remove(tile);
                
                chosenTiles.Add(tile);
            }
            else return false;
        }

        if (!_citizenPlacements.ContainsKey(cityGuid))
        {
            _citizenPlacements.Add(cityGuid, new List<CitizenPlacement>());
        }
        
        foreach (Vector2Int tile in chosenTiles)
        {
            _citizenPlacements[cityGuid].Add(new CitizenPlacement(tile, false));
            
            
            OnCitizenAddedToTile?.Invoke(cityGuid, tile);
        }
        
        HarvestState.Instance.UseCitizens(chosenTiles.Count);
        
        return true;
    }
    
    public void UseHand()
    { 
        HarvestState.Instance.UseHand();
        
        OnHandUsed?.Invoke(_citizenPlacements);
    }
    
    public void UseDiscard(Guid cityGuid)
    {
        if (HarvestState.Instance.NumRemainingDiscards > 0)
        {
            List<CitizenPlacement> citizenPlacementsBefore = new(); 
            int removedCitizens = 0;
            if (_citizenPlacements.ContainsKey(cityGuid))
            {
                foreach (CitizenPlacement placement in _citizenPlacements[cityGuid])
                {
                    citizenPlacementsBefore.Add(placement);
                }
                
                _citizenPlacements[cityGuid].Clear();

                foreach (CitizenPlacement placement in citizenPlacementsBefore)
                {
                    if (placement.IsLocked)
                    {
                        _citizenPlacements[cityGuid].Add(placement);
                    }
                    else
                    {
                        removedCitizens++;
                        OnCitizenRemovedFromTile?.Invoke(cityGuid, placement.Position);
                    }
                }
            }

            if (TryPlaceCitizensOnUnoccupiedTiles(cityGuid, removedCitizens, out List<Vector2Int> chosenTiles))
            {
                HarvestState.Instance.UseDiscard();
                
                OnDiscardUsed?.Invoke(cityGuid, citizenPlacementsBefore, _citizenPlacements[cityGuid]);
            }
            else
            {
                Debug.LogError("Could not find placements for all citizens in discard! something is bugged.");
                return;
            }
        }
    }

    public void ToggleCitizenAtTileLock(Vector2Int position)
    {
        
        CitizenPlacement placement;

        foreach (Guid cityGuid in _citizenPlacements.Keys)
        {
            placement = _citizenPlacements[cityGuid].FirstOrDefault((CitizenPlacement citizenPlacement) =>
                {
                    return citizenPlacement.Position == position;
                });

            if (placement != null)
            {
                placement.IsLocked = !placement.IsLocked;

                if (placement.IsLocked)
                {
                    OnCitizenLocked?.Invoke(position);
                }
                else
                {
                    OnCitizenUnlocked?.Invoke(position);
                }

                return;
            }
        }
        
        Debug.LogError($"Attempted to lock tile with no citizen. tile: {position}");
        return;
    }

    public bool IsCitizenAtTileLocked(Vector2Int position)
    {
        CitizenPlacement placement;

        foreach (Guid cityGuid in _citizenPlacements.Keys)
        {
            placement = _citizenPlacements[cityGuid].FirstOrDefault((CitizenPlacement citizenPlacement) =>
            {
                return citizenPlacement.Position == position;
            });

            if (placement != null)
            {
                return placement.IsLocked;
            }
        }

        return false;
    }

    private void Reset()
    {
        List<Guid> allCityGuids = MapSystem.Instance.GetAllCityGuids();

        foreach (Guid cityGuid in allCityGuids)
        {
            if (_citizenPlacements.ContainsKey(cityGuid))
            {
                List<Vector2Int> positions = new();
                foreach (CitizenPlacement placement in _citizenPlacements[cityGuid])
                {
                    positions.Add(placement.Position);
                }
                
                _citizenPlacements[cityGuid].Clear();

                foreach (Vector2Int position in positions)
                {
                    OnCitizenRemovedFromTile?.Invoke(cityGuid, position);
                }
            }
        }
        
        _citizenPlacements.Clear();
    }
    
    public bool IsCitizenOnTile(Vector2Int tilePosition)
    {
        CitizenPlacement placement;

        foreach (Guid cityGuid in _citizenPlacements.Keys)
        {
            if (_citizenPlacements[cityGuid].FirstOrDefault((CitizenPlacement citizenPlacement) =>
                {
                    return citizenPlacement.Position == tilePosition;
                }) != null)
            {
                return true;
            }
        }

        return false;
    }
}
