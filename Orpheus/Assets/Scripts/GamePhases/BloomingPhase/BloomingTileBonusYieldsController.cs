using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class BloomingTileBonusYieldsController : Singleton<BloomingTileBonusYieldsController>
{
    [SerializeField] private float cityStartAnimationTime = 1f;
    [SerializeField] private float cityEndAnimationTime = 1f;
    [SerializeField] private float tileHarvestAnimationTime = 0.25f;
    [SerializeField] private float tileProcessAnimationTime = 0.25f;
    [SerializeField] private float tileAnimationEndTime = 0.3f;
    [SerializeField] private float tileAnimationTimePerResource = 0.5f;

    public event Action OnTileBonusYieldsStart;
    public event Action OnTileBonusYieldsEnd;
    public event Action<Guid> OnCityTileBonusYieldsStart;
    public event Action<Guid> OnCityTileBonusYieldsEnd;

    public event Action<Vector2Int, int> OnTileYieldBonusChangeStart;
    public event Action<Vector2Int, int> OnTileYieldBonusChangeEnd;
    
    public void StartTileBonusYields()
    {
        Timing.RunCoroutineSingleton(TileBonusYieldsCoroutine(), this.gameObject, SingletonBehavior.Wait);
    }

    private IEnumerator<float> TileBonusYieldsCoroutine()
    {
        OnTileBonusYieldsStart?.Invoke();

        Dictionary<Vector2Int, int> currentTileYieldBonus = new();
        
        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

        foreach (Guid cityGuid in cityGuids)
        {
            OnCityTileBonusYieldsStart?.Invoke(cityGuid);
            
            List<(Vector2Int, Vector2Int, int)> allTileYieldBonuses;
            if (TerrainBonusSystem.Instance.GetTileYieldBonuses(out allTileYieldBonuses))
            {
                foreach (var bonus in allTileYieldBonuses)
                {
                    
                }
            }
            
            OnCityTileBonusYieldsEnd?.Invoke(cityGuid);
        }
    }
    

    private IEnumerator<float> HarvestCoroutine()
    {
        Dictionary<ResourceType, int> resources = PlayerResourcesSystem.Instance.GetCurrentRoundResources();

        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            if (!resources.ContainsKey(resourceType))
            {
                resources[resourceType] = 0;
            }
        }
        
        OnHarvestStart?.Invoke();
        
        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();
        
        foreach (Guid cityGuid in cityGuids)
        {
            OnCityHarvestStart?.Invoke(cityGuid);
            
            yield return OrpheusTiming.WaitForSecondsGameTime(cityStartAnimationTime);

            List<Vector2Int> cityTiles = MapSystem.Instance.GetOwnedTilesOfCity(cityGuid);

            foreach (Vector2Int cityTile in cityTiles)
            {

                List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(cityTile);
                if (CitizenController.Instance.IsCitizenOnTile(cityTile) && buildingsOnTile.Count > 0 && buildingsOnTile[0].Type != BuildingType.CityCapital)
                {
                    OnTileResourceChangeStart?.Invoke(cityTile);

                    // Dictionary<ResourceType, int> resourcesHarvested =
                    //     TileHarvestController.Instance.GetResourceChangeOnTileHarvest(cityGuid, cityTile, resources);
                    var resourcesHarvested = new Dictionary<ResourceType, int>();

                    OnTileHarvestStart?.Invoke(cityTile, resourcesHarvested);

                    foreach (KeyValuePair<ResourceType, int> resource in resourcesHarvested)
                    {
                        resources[resource.Key] += resource.Value;
                        if (resource.Value != 0)
                        {
                            yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationTimePerResource);
                        }
                    }

                    yield return OrpheusTiming.WaitForSecondsGameTime(tileHarvestAnimationTime);

                    OnTileHarvestEnd?.Invoke(cityTile);

                    //then we process the resources from this tile.
                    (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) resourcesProcessed =
                        TileHarvestController.Instance.GetResourceChangeOnTileProcess(cityGuid, cityTile, resources);
                    
                    
                    Dictionary<ResourceType, int> relicResourcesDiff = resourcesProcessed.Item1;
                    List<(RelicTypes, Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>)> relicsTriggered = RelicSystem.Instance.OnResourcesProcessed(resourcesProcessed.Item1, cityTile, out relicResourcesDiff);

                    OnTileProcessStart?.Invoke(cityTile, resourcesProcessed);

                    foreach (KeyValuePair<ResourceType, int> resource in resourcesProcessed.Item1)
                    {
                        if (resource.Value != 0)
                        {
                            resources[resource.Key] += resource.Value;
                            yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationTimePerResource);
                        }
                    }
                    
                    foreach (KeyValuePair<PersistentResourceType, int> resource in resourcesProcessed.Item2)
                    {
                        if (resource.Value != 0)
                        {
                            PlayerResourcesSystem.Instance.AddResource(resource.Key, resource.Value);
                            yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationTimePerResource);    
                        }
                    }

                    foreach (var item in relicsTriggered)
                    {
                        RelicTypes relicType = item.Item1;
                        Dictionary<ResourceType, int> resourcesDiff = item.Item2;
                        Dictionary<PersistentResourceType, int> persistentResourceDiff = item.Item3;

                        if (resourcesDiff.Count > 0)
                        {
                            foreach (KeyValuePair<ResourceType, int> resource in resourcesDiff)
                            {
                                if (resource.Value != 0)
                                {
                                    if (!resources.ContainsKey(resource.Key))
                                    {
                                        resources.Add(resource.Key, 0);
                                    }
                                    
                                    resources[resource.Key] += resource.Value;
                                    OnRelicTriggered?.Invoke(cityTile, relicType, (new Dictionary<ResourceType, int>(new KeyValuePair<ResourceType, int>[]{new KeyValuePair<ResourceType, int>(resource.Key, resource.Value)}), new()));
                                    yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationTimePerResource);
                                }
                            }
                        }
                        
                        if (persistentResourceDiff.Count > 0)
                        {
                            foreach (KeyValuePair<PersistentResourceType, int> resource in persistentResourceDiff)
                            {
                                if (resource.Value != 0)
                                {
                                    
                                    PlayerResourcesSystem.Instance.AddResource(resource.Key, resource.Value);
                                    
                                    OnRelicTriggered?.Invoke(cityTile, relicType, (new(),new Dictionary<PersistentResourceType, int>(new KeyValuePair<PersistentResourceType, int>[]{new KeyValuePair<PersistentResourceType, int>(resource.Key, resource.Value)})));
                                    yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationTimePerResource);
                                }
                            }
                        }
                    }

                    yield return OrpheusTiming.WaitForSecondsGameTime(tileProcessAnimationTime);

                    OnTileProcessEnd?.Invoke(cityTile);

                    yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationEndTime);
                    
                    OnTileResourceChangeEnd?.Invoke(cityTile);
                }
            }
            
            OnCityHarvestEnd?.Invoke(cityGuid);
            
            yield return Timing.WaitForSeconds(cityEndAnimationTime);
        }
        
        PlayerResourcesSystem.Instance.RegisterCurrentRoundResources(resources);
        
        OnHarvestEnd?.Invoke();
    }
    
}
