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

    private List<Vector2Int> _previousCitizenPositions = new();

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
                TryPlaceCitizensOnUnoccupiedTiles(allCityGuids[0], HarvestState.Instance.NumRemainingCitizens + HarvestState.Instance.NumBonusCitizensUsedThisHarvest, new(),
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
        if (TryPlaceCitizensOnUnoccupiedTiles(cityGuid, 1, _citizenPlacements[cityGuid], out List<Vector2Int> chosenTiles))
        {
            HarvestState.Instance.UseBonusCitizens(1);
            
            OnBonusCitizenUsed?.Invoke(cityGuid, chosenTiles[0]);
            return true;
        }
        else
        {
            Debug.LogError("Tried to place a bonus citizen and couldn't!");
            return false;
        }
    
    }

    private bool TryPlaceCitizensOnUnoccupiedTiles(Guid cityGuid, int numCitizensToPlace, List<CitizenPlacement> currentCitizenPlacements, out List<Vector2Int> chosenTiles)
    {
        chosenTiles = new();
        
        List<Vector2Int> allOwnedCityTiles = MapSystem.Instance.GetOwnedTilesOfCity(cityGuid);

        Vector2Int cityCenterPosition = MapSystem.Instance.GetCityCenterPosition(cityGuid);

        allOwnedCityTiles.Remove(cityCenterPosition);

        List<Vector2Int> allOwnedCityTilesCurrentlyUnoccupied = allOwnedCityTiles.Where((Vector2Int position) =>
        {
            return currentCitizenPlacements.FirstOrDefault(
                (CitizenPlacement placement) =>
                {
                    return placement.Position == position;
                }) == null;
        }).ToList();
        
        List<Vector2Int> allOwnedCityTilesPreviouslyUnoccupied = allOwnedCityTilesCurrentlyUnoccupied.Where((Vector2Int position) =>
        {
            return !_previousCitizenPositions.Contains(position);
        }).ToList();
        
        List<Vector2Int> unoccupiedCityTiles;
        if (_citizenPlacements.ContainsKey(cityGuid))
        {
            unoccupiedCityTiles = allOwnedCityTilesPreviouslyUnoccupied.Where((Vector2Int position) =>
            {
                return _citizenPlacements[cityGuid].FirstOrDefault((CitizenPlacement citizenPlacement) =>
                {
                    return citizenPlacement.Position == position;
                }) == null;
            }).ToList();
        }
        else
        {
            unoccupiedCityTiles = allOwnedCityTilesPreviouslyUnoccupied;
        }

        List<Vector2Int> additionalTileOptions = new();
        
        if (unoccupiedCityTiles.Count < numCitizensToPlace)
        {
            //first try adding tiles that were not previously occupied with a citizen
            _previousCitizenPositions.Clear();
            additionalTileOptions = allOwnedCityTilesCurrentlyUnoccupied;

            if (additionalTileOptions.Count < numCitizensToPlace - unoccupiedCityTiles.Count)
            {
                //if it's still not enough, just give all tiles
                additionalTileOptions = allOwnedCityTiles;
            }
        }

        foreach (Vector2Int position in unoccupiedCityTiles)
        {
            if (additionalTileOptions.Contains(position))
            {
                additionalTileOptions.Remove(position);
            }
        }
        
        for (int i = 0; i < numCitizensToPlace; i++)
        {
            if (unoccupiedCityTiles.Count > 0)
            {
                Vector2Int tile =
                    RandomChanceSystem.Instance.GetNextCitizenTile(unoccupiedCityTiles);
                
                unoccupiedCityTiles.Remove(tile);
                
                chosenTiles.Add(tile);
            }
            else if (additionalTileOptions.Count > 0)
            {
                Vector2Int tile =
                    RandomChanceSystem.Instance.GetNextCitizenTile(additionalTileOptions);
                
                additionalTileOptions.Remove(tile);
                
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
            //citizens start locked, unlock them to discard them
            _citizenPlacements[cityGuid].Add(new CitizenPlacement(tile, true));
            
            _previousCitizenPositions.Add(tile);
            
            OnCitizenLocked?.Invoke(tile);
            
            OnCitizenAddedToTile?.Invoke(cityGuid, tile);
        }
        
        HarvestState.Instance.UseCitizens(chosenTiles.Count);
        
        return true;
    }
    
    public void UseHand()
    { 
        HarvestState.Instance.UseHand();

        _previousCitizenPositions.Clear();
        
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

            if (TryPlaceCitizensOnUnoccupiedTiles(cityGuid, removedCitizens, citizenPlacementsBefore, out List<Vector2Int> chosenTiles))
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

    public bool AreAllCitizensLocked()
    {
        foreach (Guid cityGuid in _citizenPlacements.Keys)
        {
            foreach (var placement in _citizenPlacements[cityGuid])
            {
                if (!placement.IsLocked)
                {
                    return false;
                }
            }
        }

        return true;
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

        _previousCitizenPositions.Clear();
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
