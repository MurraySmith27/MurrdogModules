using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class BloomingHarvestController : Singleton<BloomingHarvestController>
{
    [SerializeField] private float cityStartAnimationTime = 1f;
    [SerializeField] private float cityEndAnimationTime = 1f;
    [SerializeField] private float tileHarvestAnimationTime = 0.25f;
    [SerializeField] private float tileProcessAnimationTime = 0.25f;
    [SerializeField] private float tileAnimationEndTime = 0.3f;
    [SerializeField] private float tileAnimationTimePerResource = 0.5f;

    public event Action OnHarvestStart;
    public event Action OnHarvestEnd;
    
    public event Action<Guid> OnCityHarvestStart;
    public event Action<Guid> OnCityHarvestEnd;
    
    public event Action<Vector2Int> OnTileResourceChangeStart;
    
    public event Action<Vector2Int> OnTileResourceChangeEnd;
    
    public event Action<Vector2Int, Dictionary<ResourceType, int>> OnTileHarvestStart;
    
    public event Action<Vector2Int> OnTileHarvestEnd;
    
    public event Action<Vector2Int, Dictionary<ResourceType, int>> OnTileProcessStart;
    
    public event Action<Vector2Int> OnTileProcessEnd;
    
    public event Action<Vector2Int> OnTileBonusTickStart;
    
    public event Action<Vector2Int> OnTileBonusTickEnd;
    
    public void StartHarvest()
    {
        Timing.RunCoroutineSingleton(HarvestCoroutine(), this.gameObject, SingletonBehavior.Wait);
    }

    private IEnumerator<float> HarvestCoroutine()
    {
        Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resourceType] = 0;
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
                int numTimesToHarvestTile = 1;

                if (CitizenController.Instance.IsCitizenOnTile(cityTile))
                {
                    numTimesToHarvestTile++;
                }
                
                OnTileResourceChangeStart?.Invoke(cityTile);
                
                for (int i = 0; i < numTimesToHarvestTile; i++)
                {
                    
                    Dictionary<ResourceType, int> resourcesHarvested =
                        TileHarvestController.Instance.GetResourceChangeOnTileHarvest(cityGuid, cityTile, resources);

                    OnTileHarvestStart?.Invoke(cityTile, resourcesHarvested);
                    
                    if (i > 0)
                    {
                        OnTileBonusTickStart?.Invoke(cityTile);
                    }

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
                    Dictionary<ResourceType, int> resourcesProcessed =
                        TileHarvestController.Instance.GetResourceChangeOnTileProcess(cityGuid, cityTile, resources);

                    OnTileProcessStart?.Invoke(cityTile, resourcesProcessed);

                    foreach (KeyValuePair<ResourceType, int> resource in resourcesProcessed)
                    {
                        resources[resource.Key] += resource.Value;
                        if (resource.Value != 0)
                        {
                            yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationTimePerResource);
                        }
                    }

                    yield return OrpheusTiming.WaitForSecondsGameTime(tileProcessAnimationTime);

                    OnTileProcessEnd?.Invoke(cityTile);

                    if (i > 0)
                    {
                        OnTileBonusTickEnd?.Invoke(cityTile);
                    }
                    
                    yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationEndTime);
                }
                
                OnTileResourceChangeEnd?.Invoke(cityTile);
            }
            
            OnCityHarvestEnd?.Invoke(cityGuid);
            
            yield return Timing.WaitForSeconds(cityEndAnimationTime);
        }
        
        PlayerResourcesSystem.Instance.RegisterCurrentRoundResources(resources);
        
        OnHarvestEnd?.Invoke();
    }
    
}
